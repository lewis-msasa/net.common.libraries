using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.UnitOfWork
{
    public interface IUnitOfWork<T> : IDisposable where T : class, IEntity
    {
        Task<int> Commit(CancellationToken cancellationToken);
        void Rollback();
        IRepository<T> Repository();
    }
}
