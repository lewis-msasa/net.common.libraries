using Common.Libraries.Services.EFCore.Repositories;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Common.Libraries.Services.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore.UnitOfWork
{
    
    public class UnitOfWork<T,Context> : IDisposable,IUnitOfWork<T> where T: class, IEntity where Context : DbContext
    {
        private readonly Context _dbContext;
        private readonly IRepository<T> _repository;
        public UnitOfWork(Context dbContext)
        {
            _repository = new UnitOfWorkRepository<T, Context>(dbContext);
            _dbContext = dbContext;
        }
        public Task<int> Commit(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Rollback()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        public IRepository<T> Repository() 
        {
            return _repository;
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
