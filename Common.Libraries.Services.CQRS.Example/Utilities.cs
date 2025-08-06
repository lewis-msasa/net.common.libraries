using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Example
{
    public interface ICacheStrategy<TRequest, TResponse>
    {
        Task<TResponse?> TryGetAsync(TRequest request);
        Task SetAsync(TRequest request, TResponse response);
    }

    public interface IAuditStrategy<TRequest>
    {
        Task LogStartAsync(TRequest request);
        Task LogEndAsync(TRequest request);
    }

    public interface IPermissionStrategy<TRequest>
    {
        Task<bool> IsAuthorizedAsync(TRequest request);
    }

    public interface IMetricsStrategy<TRequest, TResponse>
    {
        Task<TResult> MeasureAsync<TResult>(TRequest request, Func<Task<TResult>> next);
    }

}
