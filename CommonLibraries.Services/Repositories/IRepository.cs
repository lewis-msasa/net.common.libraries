
using Common.Libraries.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
         string[] includeString = null,
          bool disableTracking = true, CancellationToken cancellationToken = default);

       


        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);
     

        Task<IReadOnlyList<T>> GetPaginatedAsync(int page,
            int size, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default);

       

        Task<IReadOnlyList<T>> GetPaginatedByCondtionAsync(Expression<Func<T, bool>> predicate,
            int page, int size,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string[] includeString = null,
           bool disableTracking = true, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
