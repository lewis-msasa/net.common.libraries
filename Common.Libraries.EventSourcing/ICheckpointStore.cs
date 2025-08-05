using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
    public interface IProjector
    {
        string Name { get; }  
        Task ProjectAsync(object @event, CancellationToken cancellationToken);
    }
    public interface ICheckpointStore
    {
        Task<long?> GetCheckpoint(string projector,CancellationToken cancellationToken);
        Task StoreCheckpoint(string projector, long? checkpoint, CancellationToken cancellationToken);
    }
}