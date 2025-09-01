using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification.EFCore
{
    using Microsoft.EntityFrameworkCore;

    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // Filtering
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Ordering
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Paging
            if (spec.IsPagingEnabled)
            {
                if (spec.Skip.HasValue) query = query.Skip(spec.Skip.Value);
                if (spec.Take.HasValue) query = query.Take(spec.Take.Value);
            }

            return query;
        }
    }

}
