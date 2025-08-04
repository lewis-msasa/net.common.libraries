using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public class ProjectionWorker : BackgroundService
    {
        private readonly ISubscription _projection;
        private readonly ICheckpointStore _checkpointStore;
        private readonly IEventDeserializer _deserializer;
        private readonly IService<Event, EventDto> _eventService;
        private readonly IService<Snapshot, SnapshotDto> _eventSnapshotService;

        public ProjectionWorker(ISubscription projection, ICheckpointStore checkpointStore, 
            IEventDeserializer deserializer, 
            IService<Event, EventDto> eventService, IService<Snapshot, SnapshotDto> eventSnapshotService)
        {
            _projection = projection;
            _checkpointStore = checkpointStore;
            _deserializer = deserializer;
            _eventService = eventService;
            _eventSnapshotService = eventSnapshotService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
