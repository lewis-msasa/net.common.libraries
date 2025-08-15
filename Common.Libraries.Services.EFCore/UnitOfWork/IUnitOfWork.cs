using Common.Libraries.Services.EFCore.Repositories;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Common.Libraries.Services.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore.UnitOfWork
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    public class UnitOfWork<Context> : IDisposable, IUnitOfWork where Context : DbContext
    {
        private readonly Context _context;
        private IDbContextTransaction _transaction;
        private IUnitOfWorkRepository _repository;

        public UnitOfWork(Context context)
        {
            _context = context;
            _repository = new UnitOfWorkRepository(context);
        }
        public IUnitOfWorkRepository Repository()
        {
            return _repository;
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already started.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
