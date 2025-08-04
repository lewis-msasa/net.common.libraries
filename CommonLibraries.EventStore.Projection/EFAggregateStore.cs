using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public class Event : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class EventDto : IDTO
    {
 
        public int RecordId { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class Snapshot : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string SnapshotData { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class SnapshotDto : IDTO
    {
      
        public int RecordId { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string SnapshotData { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class EFAggregateStore : IAggregateStore, ISaveSnapshot
    {
        private readonly IService<Event, EventDto> _eventService;
        private readonly IService<Snapshot, SnapshotDto> _eventSnapshotService;
        private readonly IEventDeserializer _deserializer;

        public EFAggregateStore(IService<Event, EventDto> eventService, IService<Snapshot, SnapshotDto> eventSnapshotService, IEventDeserializer deserializer)
        {
            _eventService = eventService;
            _eventSnapshotService = eventSnapshotService;
            _deserializer = deserializer;
        }

        public async Task<bool> Exists<T>(AggregateId<T> aggregateId) where T : AggregateRoot
        {
            var eventItem = await _eventService.GetOneAsync(e => e.AggregateId == aggregateId.Value);
            return (eventItem != null);
        }

        public async Task<T> Load<T>(AggregateId<T> aggregateId) where T : AggregateRoot
        {
            var aggregateType = typeof(T).Name;

            var snapshot = await _eventSnapshotService.GetOneAsync(s => s.AggregateId == aggregateId.Value && s.AggregateType == aggregateType, o => o.OrderByDescending(t => t.Timestamp));
            var aggregate = (T)Activator.CreateInstance(typeof(T), true);

            int snapshotVersion = -1;

            if (snapshot != null)
            {
                snapshotVersion = snapshot.Version;
                var state = JsonSerializer.Deserialize(snapshot.SnapshotData, typeof(T));
                var field = typeof(T).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic);
                field?.SetValue(aggregate, state);
                aggregate.Version = snapshot.Version;
            }


            var events = await _eventService.GetByConditionAsync(e => e.AggregateId == aggregateId.Value && e.AggregateType == typeof(T).Name);
            if (!events.Any())
                return null;

            var domainEvents = events.Select(e => _deserializer.Deserialize(e.EventData, e.EventType));
            aggregate.Load(domainEvents);

            return aggregate;

            
        }

        public async Task Save<T>(T aggregate) where T : AggregateRoot
        {
            var changes = aggregate.GetChanges().ToList();

            var eventEntities = changes.Select((e, index) => new Event
            {
                AggregateId = aggregate.Id,
                AggregateType = typeof(T).Name,
                EventType = e.GetType().Name,
                EventData = JsonSerializer.Serialize(e, e.GetType()), 
                Version = aggregate.Version + 1 + index,
                Timestamp = DateTime.UtcNow
            });
            await _eventService.CreateManyAsync(eventEntities.ToList());

            aggregate.ClearChanges();

        }

        public async Task SaveSnapshot<T>(T aggregate) where T : AggregateRoot
        {
            var snapshot = new Snapshot
            {
                AggregateId = aggregate.Id,
                AggregateType = typeof(T).Name,
                Version = aggregate.Version,
                SnapshotData = JsonSerializer.Serialize(aggregate, aggregate.GetType()), // assuming state is internal
                Timestamp = DateTime.UtcNow
            };
            await _eventSnapshotService.UpdateAsync(snapshot);

        }
    }
}
