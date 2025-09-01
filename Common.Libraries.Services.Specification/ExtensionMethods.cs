using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification
{

    using System.Linq.Expressions;

    public static class SpecificationExtensions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            var param = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(left.Criteria, param),
                Expression.Invoke(right.Criteria, param));

            return new ExpressionSpecification<T>(Expression.Lambda<Func<T, bool>>(body, param));
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            var param = Expression.Parameter(typeof(T));

            var body = Expression.OrElse(
                Expression.Invoke(left.Criteria, param),
                Expression.Invoke(right.Criteria, param));

            return new ExpressionSpecification<T>(Expression.Lambda<Func<T, bool>>(body, param));
        }
    }

    public class ExpressionSpecification<T> : Specification<T>
    {
        private readonly Expression<Func<T, bool>> _criteria;

        public ExpressionSpecification(Expression<Func<T, bool>> criteria)
        {
            _criteria = criteria;
        }

        public override Expression<Func<T, bool>> Criteria => _criteria;
    }

}
