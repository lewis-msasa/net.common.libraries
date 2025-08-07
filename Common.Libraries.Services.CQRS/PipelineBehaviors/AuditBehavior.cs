using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.PipelineBehaviors
{
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
       
        private readonly IServiceProvider _provider;

        public AuditBehavior(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            
            if (_provider.TryGetService<IAuditStrategy<TRequest>>(out var auditStrategy))
            {

                await auditStrategy.LogStartAsync(request);
                var response = await next();
                await auditStrategy.LogEndAsync(request);
                return response;
            }
            
                return await next();
            
        }
    }
}
