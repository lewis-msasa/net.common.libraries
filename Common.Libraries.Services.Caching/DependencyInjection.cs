using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCacheDependencies(this IServiceCollection services, bool UseMemory=true, string? redisAddress = null, string? redisInstance = null)
        {
            if (UseMemory)
            {
                services.AddMemoryCache();
                services.AddScoped<IAppCache, InMemoryAppCache>();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisAddress;
                    options.InstanceName = redisInstance;
                });
                services.AddScoped<IAppCache, DistributedAppCache>();

            }
            return services;
        }
    }
}
