using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification
{
    public interface IRepository<T> where T : class, ISpecEntity
    {
        Task<List<T>> FindAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default!);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken);
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default!);
    }
}
