using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.PipelineBehaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
      
        private readonly IServiceProvider _provider;

        public CachingBehavior(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            if (_provider.TryGetService<ICacheStrategy<TRequest, TResponse>>(out var cacheStrategy))
            {
            
                var cachedResponse = await cacheStrategy.TryGetAsync(request);
            }
            return await next();
        }
    }
}
