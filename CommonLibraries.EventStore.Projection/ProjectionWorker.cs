using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public class ProjectionWorker<T> : BackgroundService where T : class, IEntity
    {
        private  ISubscription<T> _projection;
        private  ICheckpointStore _checkpointStore;
        private IEventDeserializer _deserializer;
        private IService<Event, EventDto> _eventService;
        private IService<Snapshot, SnapshotDto> _eventSnapshotService;
        private IServiceScopeFactory _scopeFactory;

        public ProjectionWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            using var scope = _scopeFactory.CreateScope();
            _checkpointStore = scope.ServiceProvider.GetRequiredService<ICheckpointStore>();
            _projection = scope.ServiceProvider.GetRequiredService<ISubscription<T>>();
            _deserializer = scope.ServiceProvider.GetRequiredService<IEventDeserializer>();
            _eventService = scope.ServiceProvider.GetRequiredService<IService<Event, EventDto>>();  
            _eventSnapshotService = scope.ServiceProvider.GetRequiredService<IService<Snapshot, SnapshotDto>>();
            long lastCheckpoint = await _checkpointStore.GetCheckpoint() ?? 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                // Load new events since last checkpoint
                var events = await _eventService.GetPaginatedByConditionAsync(e => e.RecordId > lastCheckpoint,1,100, o => o.OrderByDescending(o => o.RecordId));

                if (!events.Any())
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                foreach (var e in events)
                {
                    var @event = _deserializer.Deserialize(e.EventData, e.EventType);
                    await _projection.Project(@event); // Update the read model
                    lastCheckpoint = e.RecordId;
                    await _checkpointStore.StoreCheckpoint(lastCheckpoint);
                }
            }
        }
    }
}
