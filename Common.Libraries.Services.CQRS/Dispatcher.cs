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
        Task Send(IRequest request, CancellationToken cancellationToken = default);
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
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

        public async Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            var handler = _provider.GetService(handlerType);

            var behaviors = _provider.GetServices(typeof(IVoidPipelineBehavior<>)
                   .MakeGenericType(requestType))
               .Cast<object>()
               .ToList();

            if (handler == null)
                throw new InvalidOperationException($"Handler for {requestType.Name} not found");

            RequestHandlerDelegate handlerDelegate = () =>
            {
                var method = handlerType.GetMethod("Handle");
                return (Task)method.Invoke(handler, [request, cancellationToken]);
            };

            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = handlerDelegate;
                handlerDelegate = () => ((dynamic)behavior).Handle((dynamic)request, next);
            }
            await handlerDelegate();
        }
        

    }

}
