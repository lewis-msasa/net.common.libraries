using System.Diagnostics;

namespace Common.Libraries.Services.CQRS.TestApi
{
    public class BasicMetrics<TRequest, TResponse> : IMetricsStrategy<TRequest, TResponse>
    {
        private readonly ILogger<BasicMetrics<TRequest, TResponse>> _logger;

        public BasicMetrics(ILogger<BasicMetrics<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> MeasureAsync<TResult>(TRequest request, Func<Task<TResult>> next)
        {
            var sw = Stopwatch.StartNew();
            var result = await next();
            sw.Stop();

            _logger.LogInformation("Executed {Request} in {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
            return result;
        }
    }

}
