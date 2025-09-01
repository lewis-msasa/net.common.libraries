using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification.EFCore
{
    internal class EFRepository<T, Context> : IRepository<T> where T : class, ISpecEntity where Context : DbContext
    {
        private readonly Context _dbContext;

        public EFRepository(Context dbContext)
        {
            _dbContext = dbContext;
        }
        //var recentOrdersSpec = new Specification<Order>()
        //.Where(o => o.OrderDate >= DateTime.UtcNow.AddDays(-7))
        //.AddInclude(o => o.Customer)
        //.ApplyPaging(0, 5);
        public async Task<List<T>> FindAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken =default!)
        {
            var query = ApplySpecification(spec);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken =default!)
        {
            var query = ApplySpecification(spec);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default!)
        {
            var query = ApplySpecification(spec);
            return await query.CountAsync(cancellationToken);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T>? spec = null)
        {
            if(spec == null)
            {
                return _dbContext.Set<T>().AsQueryable();
            }
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}
