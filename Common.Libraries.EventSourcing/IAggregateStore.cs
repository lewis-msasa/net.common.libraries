using System;
using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
    public interface IAggregateStore<T, TSnapshot> where T : AggregateRoot<TSnapshot>, new() where TSnapshot : ISnapshot
    {
        Task<bool> Exists(AggregateId<T, TSnapshot> aggregateId);

        Task Save(T aggregate);

        Task<T> Load(AggregateId<T, TSnapshot> aggregateId);

       
    }
    public interface ISaveSnapshot<T, TSnapshot> where T : AggregateRoot<TSnapshot>, new() where TSnapshot : ISnapshot
    {
        Task SaveSnapshot(T aggregate);
    }

    public interface IFunctionalAggregateStore
    {
        Task Save<T>(long version, AggregateState<T>.Result update)
            where T : class, IAggregateState<T>, new();

        Task<T> Load<T>(Guid id)
            where T : IAggregateState<T>, new();
    }
}