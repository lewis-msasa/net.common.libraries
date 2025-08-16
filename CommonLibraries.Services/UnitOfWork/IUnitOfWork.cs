using Common.Libraries.Services.EFCore.UnitOfWork;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.UnitOfWork
{
    public interface IUnitOfWork<Context> : IDisposable where Context : DbContext
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IUnitOfWorkRepository<T> Repository<T>() where T : class, IEntity;
    }
}
