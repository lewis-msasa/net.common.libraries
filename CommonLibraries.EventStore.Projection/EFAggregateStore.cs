using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
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
    public class EFAggregateStore<T,TSnapshot> : IAggregateStore<T,TSnapshot>, ISaveSnapshot<T,TSnapshot> where T : AggregateRoot<TSnapshot>, new() where TSnapshot : class,ISnapshot,IEntity
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<TSnapshot> _eventSnapshotRepository;
        private readonly IEventDeserializer _deserializer;

        public EFAggregateStore(IRepository<Event> eventRepository, IRepository<TSnapshot> eventSnapshotRepository, IEventDeserializer deserializer)
        {
            _eventRepository = eventRepository;
            _eventSnapshotRepository = eventSnapshotRepository;
            _deserializer = deserializer;
        }

        public async Task<bool> Exists(AggregateId<T,TSnapshot> aggregateId)
        {
            var eventItem = await _eventRepository.GetOneAsync(e => e.AggregateId == aggregateId.Value);
            return (eventItem != null);
        }

        public async Task<T> Load(AggregateId<T,TSnapshot> aggregateId)
        {
            var aggregateType = typeof(T).Name;

            var snapshot = await _eventSnapshotRepository.GetOneAsync(s => s.AggregateId == aggregateId.Value, o => o.OrderByDescending(t => t.Timestamp));
            var aggregate = (T)Activator.CreateInstance(typeof(T), true);
            var events = await _eventRepository.GetAsync(e => e.AggregateId == aggregateId.Value && e.AggregateType == typeof(T).Name);
            if (!events.Any())
                return null;
            var domainEvents = events.Select(e => _deserializer.Deserialize(e.EventData, e.EventType));
            
            int snapshotVersion = snapshot?.Version ?? -1;
            if(snapshot != null)
                   aggregate.LoadFromHistory(snapshot, domainEvents);
            else
                   aggregate.Load(domainEvents);
            aggregate.Version = snapshotVersion;

            return aggregate;

            
        }

        public async Task Save(T aggregate)
        {
            var changes = aggregate.GetChanges().ToList();

            var events = await _eventRepository.GetAsync(t => t.AggregateId == aggregate.Id);
            var currentVersion = events.MaxBy(t => t.Version)?.Version ?? -1;
            aggregate.Version = currentVersion;
            var eventEntities = changes.Select((e, index) => new Event
            {
                AggregateId = aggregate.Id,
                AggregateType = typeof(T).Name,
                EventType = e.GetType().Name,
                EventData = JsonSerializer.Serialize(e, e.GetType()), 
                Version = currentVersion + 1 + index,
                Timestamp = DateTime.UtcNow
            });
            foreach(var entiy in eventEntities.ToList())
            {
                await _eventRepository.AddAsync(entiy);
            }
            if (ShouldSnapshot(aggregate))
            {
                await SaveSnapshot(aggregate);
            }

            aggregate.ClearChanges();

        }

        public async Task SaveSnapshot(T aggregate)
        {
            var snapshot = aggregate.CreateSnapShot();
            await _eventSnapshotRepository.UpdateAsync(snapshot);

        }
        private bool ShouldSnapshot(T aggregate)
        {
            //every 100
            return aggregate.Version % 100 == 0;
        }
    }
}
