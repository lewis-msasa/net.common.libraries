using System;
using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
    public interface IAggregateStore<T, TSnapshot> where T : AggregateRoot<TSnapshot> where TSnapshot : ISnapshot
    {
        Task<bool> Exists(AggregateId<T, TSnapshot> aggregateId, CancellationToken cancellationToken = default!);

        Task Save(T aggregate, CancellationToken cancellationToken = default!);

        Task<T> Load(AggregateId<T, TSnapshot> aggregateId, CancellationToken cancellationToken = default!);

       
    }
    public interface ISaveSnapshot<T, TSnapshot> where T : AggregateRoot<TSnapshot> where TSnapshot : ISnapshot
    {
        Task SaveSnapshot(T aggregate, CancellationToken cancellationToken = default!);
    }

    public interface IFunctionalAggregateStore
    {
        Task Save<T>(long version, AggregateState<T>.Result update, CancellationToken cancellationToken = default!)
            where T : class, IAggregateState<T>, new();

        Task<T> Load<T>(Guid id, CancellationToken cancellationToken = default!)
            where T : IAggregateState<T>, new();
    }
}