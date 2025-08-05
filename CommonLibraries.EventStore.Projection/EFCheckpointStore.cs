using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public class Checkpoint : IEntity
    {
        public string Id { get; set; }
        public long? Position { get; set; }
    }
    public class CheckpointDto : IDTO
    {
        public string Id { get; set; }
        public long? Position { get; set; }
    }
    public class EFCheckpointStore : ICheckpointStore
    {
        private readonly IRepository<Checkpoint> _repository;
        const string CheckpointStreamPrefix = "checkpoint:";
        public EFCheckpointStore(IRepository<Checkpoint> repository
            )
        {
            _repository = repository;
        }

        public async Task<long?> GetCheckpoint(string projector,CancellationToken cancellationToken)
        {
            var checkPoint = await _repository.GetOneAsync(t => t.Id == CheckpointStreamPrefix + projector);
            return checkPoint?.Position;
        }

        public async Task StoreCheckpoint(string projector,long? position, CancellationToken cancellationToken)
        {
            var checkPoint = await _repository.GetOneAsync(t => t.Id == CheckpointStreamPrefix +  projector);
            if (checkPoint == null)
            {
                checkPoint = new Checkpoint
                {
                    Id = CheckpointStreamPrefix +  projector,
                    Position = position ?? 0,
                };
                await _repository.AddAsync(checkPoint);
                return;
                
            }
            checkPoint.Position = position; 
            await _repository.UpdateAsync(checkPoint);

        }
    }
}
