using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.PipelineBehaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IPermissionStrategy<TRequest> _strategy;

        public AuthorizationBehavior(IPermissionStrategy<TRequest> strategy)
        {
            _strategy = strategy;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            await _strategy.IsAuthorizedAsync(request);
            return await next();
        }
    }

}
