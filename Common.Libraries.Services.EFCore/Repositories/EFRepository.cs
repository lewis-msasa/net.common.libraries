
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore.Repositories
{
    public class EFRepository<T, Context> : IDisposable,IRepository<T> where T : class, IEntity where Context : DbContext
    {
        private readonly Context _dbContext;

        public EFRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string[] includeString = null,
            bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            if (includeString != null)
            {

                foreach (var inc in includeString)
                {
                    query = query.Include(inc);
                }
            }
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            if (!disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query?.FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            var result = await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {

            _dbContext.Set<T>().Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }
        



        public async Task<IReadOnlyList<T>> GetPaginatedByCondtionAsync(Expression<Func<T, bool>> predicate, int page, int size, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            page = page != 0 ? page - 1 : page;
            var query = _dbContext.Set<T>().Where(predicate).Skip(page * size).Take(size);
            if (includeString != null)
            {

                foreach (var inc in includeString)
                {
                    query = query.Include(inc);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }
        

        public async Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Update(entity);

            var result = await _dbContext.SaveChangesAsync();
            return result;
        }

       

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            if (includeString != null)
            {

                foreach (var inc in includeString)
                {
                    query = query.Include(inc);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetPaginatedAsync(int page, int size, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string[] includeString = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            
            page = page != 0 ? page - 1 : page;
            var skip = page * size;
            IQueryable<T> query = _dbContext.Set<T>().Skip(skip).Take(size);
            if (includeString != null)
            {

                foreach (var inc in includeString)
                {
                    query = query.Include(inc);
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!disableTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
           if(predicate != null)
             return await _dbContext.Set<T>().CountAsync(predicate);
           else
             return await _dbContext.Set<T>().CountAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
