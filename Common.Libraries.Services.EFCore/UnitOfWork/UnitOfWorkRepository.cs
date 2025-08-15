using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore.UnitOfWork
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly DbContext _dbContext;

        public UnitOfWorkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task<int> DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {

            _dbContext.Set<T>().Remove(entity);
            return 1;
        }

        public async Task<int> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
        {
            _dbContext.Set<T>().Update(entity);
            return 1;
        }


    }
}
