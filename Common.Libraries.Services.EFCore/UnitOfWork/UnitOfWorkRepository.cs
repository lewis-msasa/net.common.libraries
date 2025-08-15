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
    public class UnitOfWorkRepository<T,Context> : IUnitOfWorkRepository<T> where T : class, IEntity where Context: DbContext
    {
        private readonly Context _dbContext;

        public UnitOfWorkRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {

            _dbContext.Set<T>().Remove(entity);
            return 1;
        }

        public async Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Update(entity);
            return 1;
        }


    }
}
