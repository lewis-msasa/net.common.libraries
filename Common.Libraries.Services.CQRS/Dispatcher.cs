using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS
{
    public interface IDispatcher
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        //Task<TResponse> SendWithPipelines<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
    public record RequestContext(
        object Request,
        object Handler,
        MethodInfo HandlerMethod,
        CancellationToken CancellationToken,
        IServiceProvider Provider
    );

    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _provider;

        public Dispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            var handler = _provider.GetRequiredService(handlerType);

            var behaviors = _provider.GetServices(typeof(IPipelineBehavior<,>)
                    .MakeGenericType(requestType, typeof(TResponse)))
                .Cast<object>()
                .ToList();

            RequestHandlerDelegate<TResponse> handlerDelegate = () =>
            {
                var method = handlerType.GetMethod("Handle");
                return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken]);
            };

            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = handlerDelegate;
                handlerDelegate = () => ((dynamic)behavior).Handle((dynamic)request, next);
            }

            return await handlerDelegate();
        }
        //public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        //{
        //    var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        //    var handler = _provider.GetRequiredService(handlerType);
        //    var method = handlerType.GetMethod("Handle")!;

        //    var context = new RequestContext(request, handler, method, cancellationToken, _provider);
        //    return await ExecuteWithBehaviors<TResponse>(context);
        //}
        //private async Task<TResponse> ExecuteWithBehaviors<TResponse>(RequestContext ctx)
        //{
        //    var requestType = ctx.Request.GetType();
        //    var responseType = typeof(TResponse);

        //    // Resolve optional services
        //    var cacheType = typeof(ICacheStrategy<,>).MakeGenericType(requestType, responseType);
        //    var cacheStrategy = ctx.Provider.GetService(cacheType);
        //    var auditType = typeof(IAuditStrategy<>).MakeGenericType(requestType);
        //    var auditStrategy = ctx.Provider.GetService(auditType);
        //    var permissionType = typeof(IPermissionStrategy<>).MakeGenericType(requestType);
        //    var permissionStrategy = ctx.Provider.GetService(permissionType);
        //    var metricsStrategy = ctx.Provider.GetService(typeof(IMetricsStrategy<,>).MakeGenericType(requestType, responseType));

        //    // Permissions
        //    var itIs = (permissionStrategy is IPermissionStrategy<IRequest<TResponse>> auth);
        //    if (permissionStrategy != null)
        //    {
        //        var method = permissionType.GetMethod("IsAuthorizedAsync");
        //        var authorized = await (Task<bool>)method.Invoke(permissionStrategy, [(IRequest<TResponse>)ctx.Request]);
        //        if(!authorized)
        //          throw new UnauthorizedAccessException($"Unauthorized: {requestType.Name}");
        //    }

        //    // Audit start
        //    if (auditStrategy != null)
        //    {
        //        var auditStartMethod = auditType.GetMethod("LogStartAsync");
        //        await (auditStartMethod?.Invoke(auditStrategy, [(IRequest<TResponse>)ctx.Request]) as Task)!;
        //    }
                

        //    // Caching
        //    if (cacheStrategy != null)
        //    {
        //        var cacheMethod = cacheType.GetMethod("TryGetAsync");
        //        var cached = await (Task<TResponse?>)cacheMethod.Invoke(cacheStrategy, [(IRequest<TResponse>)ctx.Request]);
        //        if (cached is not null) return cached;
        //    }

        //    // Execute with optional metrics
        //    var executeHandler = async () =>
        //    {
        //        var result = ctx.HandlerMethod.Invoke(ctx.Handler, new object[] { ctx.Request, ctx.CancellationToken });
        //        return await (Task<TResponse>)result!;
        //    };

        //    TResponse response;
        //    if (metricsStrategy is IMetricsStrategy<IRequest<TResponse>, TResponse> metrics)
        //    {
        //        response = await metrics.MeasureAsync((IRequest<TResponse>)ctx.Request, executeHandler);
        //    }
        //    else
        //    {
        //        response = await executeHandler();
        //    }

        //    // Save to cache if applicable
        //    if (cacheStrategy is ICacheStrategy<IRequest<TResponse>, TResponse> cacheSet)
        //    {
        //        await cacheSet.SetAsync((IRequest<TResponse>)ctx.Request, response);
        //    }

        //    // Audit end
        //    if (auditStrategy is IAuditStrategy<IRequest<TResponse>> auditEnd)
        //        await auditEnd.LogEndAsync((IRequest<TResponse>)ctx.Request);

        //    return response;
        //}

    }

}
