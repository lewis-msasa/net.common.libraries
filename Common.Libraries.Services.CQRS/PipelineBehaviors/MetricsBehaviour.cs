using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.PipelineBehaviors
{
    public class MetricsBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        
        private readonly IServiceProvider _provider;

        public MetricsBehaviour(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {

            if (_provider.TryGetService<IMetricsStrategy<TRequest, TResponse>>(out var metricsStrategy))
            {
                return await metricsStrategy.MeasureAsync(request, () => next());

            }
            return await next();
            
            

        }
    }
}
