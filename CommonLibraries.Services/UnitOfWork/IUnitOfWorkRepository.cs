using Common.Libraries.Services.Entities;

namespace Common.Libraries.Services.EFCore.UnitOfWork
{
    public interface IUnitOfWorkRepository<T> where T : class, IEntity
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
}