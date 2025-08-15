using Common.Libraries.Services.Entities;

namespace Common.Libraries.Services.EFCore.UnitOfWork
{
    public interface IUnitOfWorkRepository
    {
        Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
        Task<int> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
    }
}