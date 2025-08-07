
using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Services
{
    public interface IService<T, TD> where T : IEntity where TD : IDTO 
    {
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
       

        Task<ICollection<T>> GetPaginatedAsync(int page, int size,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default);

        Task<ICollection<T>> CreateManyAsync(ICollection<T> entity, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> UpdateAsync(T dto, CancellationToken cancellationToken = default);

        Task<int> UpdateManyAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
        Task<ICollection<T>> GetAsync(CancellationToken cancellationToken = default);

        Task<ICollection<T>> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<ICollection<T>> GetPaginatedByConditionAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int size,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default
            );
    }
    public interface IServiceWithMapping<T, TD> where T : IEntity where TD : IDTO
    {
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<TD> GetOneAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<T,TD> mapping = null,
            string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);
        Task<TD> CreateAsync(T entity, Func<T, TD> mapping, CancellationToken cancellationToken = default);


        Task<ICollection<TD>> GetPaginatedAsync(int page, int size,
            Func<ICollection<T>, ICollection<TD>> mapping = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default);

        Task<ICollection<TD>> CreateManyAsync(ICollection<T> entity, Func<ICollection<T>, ICollection<TD>> mapping = null, CancellationToken cancellationToken = default);
        Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> UpdateAsync(T dto, CancellationToken cancellationToken = default);

        Task<int> UpdateManyAsync(ICollection<T> entities, CancellationToken cancellationToken = default);
        Task<ICollection<TD>> GetAsync(Func<ICollection<T>, ICollection<TD>> mapping = null, CancellationToken cancellationToken = default);

        Task<ICollection<TD>> GetByConditionAsync(Expression<Func<T, bool>> predicate, Func<ICollection<T>, ICollection<TD>> mapping = null, CancellationToken cancellationToken = default);

        Task<ICollection<TD>> GetPaginatedByConditionAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int size,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<ICollection<T>, ICollection<TD>> mapping = null,
            string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default
            );
    }
}