using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Libraries.EventSourcing;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace Common.Libraries.EventStore
{
    public class EsAggregateStore<T, TSnapshot> : IAggregateStore<T, TSnapshot> where T : AggregateRoot<TSnapshot>, new () where TSnapshot : class,ISnapshot
    {
        
        private readonly ILogger<EsAggregateStore<T,TSnapshot>> _logger;
        readonly IEventStoreConnection _connection;

        public EsAggregateStore(IEventStoreConnection connection,
            ILogger<EsAggregateStore<T,TSnapshot>> logger)
        { 
            _connection = connection;
            _logger = logger;
        }

        public async Task Save(T aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var streamName = GetStreamName(aggregate);
            var changes = aggregate.GetChanges().ToArray();

            foreach (var change in changes)
               _logger.LogDebug("Persisting event {event}", change.ToString());

            await _connection.AppendEvents(streamName, aggregate.Version, changes);

            aggregate.ClearChanges();
        }

        public async Task<T> Load(AggregateId<T,TSnapshot> aggregateId)
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            var stream = GetStreamName(aggregateId);
            var aggregate = (T) Activator.CreateInstance(typeof(T), true);

            var page = await _connection.ReadStreamEventsForwardAsync(
                stream, 0, 1024, false
            );

            _logger.LogDebug("Loading events for the aggregate {aggregate}", aggregate.ToString());

            aggregate.Load(
                page.Events.Select(
                        resolvedEvent => resolvedEvent.Deserialze()
                    )
                    .ToArray()
            );

            return aggregate;
        }

        public async Task<bool> Exists(AggregateId<T,TSnapshot> aggregateId) 
        {
            var stream = GetStreamName(aggregateId);
            var result = await _connection.ReadEventAsync(stream, 1, false);
            return result.Status != EventReadStatus.NoStream;
        }

        static string GetStreamName(AggregateId<T,TSnapshot> aggregateId) 
            => $"{typeof(T).Name}-{aggregateId}";

        static string GetStreamName(T aggregate)
            => $"{typeof(T).Name}-{aggregate.Id.ToString()}";

     
    }
}