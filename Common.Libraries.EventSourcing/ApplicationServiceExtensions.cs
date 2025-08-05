using System;
using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
    public static class ApplicationServiceExtensions
    {
        public static async Task HandleUpdate<T,TSnapshot>(
            this IApplicationService service,
            IAggregateStore<T,TSnapshot> store,
            AggregateId<T,TSnapshot> aggregateId,
            Action<T> operation)
            where T : AggregateRoot<TSnapshot>, new() where TSnapshot : ISnapshot
        {
            var aggregate = await store.Load(aggregateId);

            if (aggregate == null)
                throw new InvalidOperationException($"Entity with id {aggregateId} cannot be found");

            operation(aggregate);
            await store.Save(aggregate);
        }
    }
}