using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Libraries.EventSourcing;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Common.Libraries.EventStore
{
    public class EsCheckpointStore : ICheckpointStore
    {
        const string CheckpointStreamPrefix = "checkpoint:";
        readonly IEventStoreConnection _connection;
        public EsCheckpointStore(
            IEventStoreConnection connection
            )
        {
            _connection = connection;
        }

        public async Task<long?> GetCheckpoint(string projector, CancellationToken cancellationToken)
        {
            var slice = await _connection
                .ReadStreamEventsBackwardAsync(CheckpointStreamPrefix + projector, -1, 1, false);

            var eventData = slice.Events.FirstOrDefault();

            if (eventData.Equals(default(ResolvedEvent)))
            {
                await StoreCheckpoint(projector, AllCheckpoint.AllStart?.CommitPosition, cancellationToken);
                await SetStreamMaxCount(projector);
                return null;
            }

            return eventData.Deserialze<Checkpoint>()?.Position;
        }

        public Task StoreCheckpoint(string projector,long? checkpoint,CancellationToken cancellationToken)
        {
            var @event = new Checkpoint {Position = checkpoint};
            
            var preparedEvent =
                new EventData(
                    Guid.NewGuid(),
                    "$checkpoint",
                    true,
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(@event)
                    ),
                    null
                );

            return _connection.AppendToStreamAsync(
                CheckpointStreamPrefix + projector,
                ExpectedVersion.Any,
                preparedEvent
            );
        }

        async Task SetStreamMaxCount(string projector)
        {
            var metadata = await _connection.GetStreamMetadataAsync(CheckpointStreamPrefix + projector);

            if (!metadata.StreamMetadata.MaxCount.HasValue)
                await _connection.SetStreamMetadataAsync(
                    CheckpointStreamPrefix + projector, ExpectedVersion.Any,
                    StreamMetadata.Create(1)
                );
        }

        class Checkpoint
        {
            public long? Position { get; set; }
        }
    }
}