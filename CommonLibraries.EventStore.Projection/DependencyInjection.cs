using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEventStorePersistence(this IServiceCollection services, string projectionName)
        {
            services.AddScoped<IAggregateStore, EFAggregateStore>();
            services.AddScoped<ISaveSnapshot, EFAggregateStore>();
            //services.AddScoped<ICheckpointStore, EFCheckpointStore>();
            services.AddScoped<ICheckpointStore>(sp =>
                      new EFCheckpointStore(sp.GetRequiredService<IRepository<Checkpoint>>(), projectionName));
            services.AddHostedService<ProjectionWorker>();

            return services;
        }
    }
}
