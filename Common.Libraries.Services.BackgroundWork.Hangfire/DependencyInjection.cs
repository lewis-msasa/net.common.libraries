using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.BackgroundWork.Hangfire
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHangfireDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            var connectionString = Configuration.GetConnectionString("HangfireConnection");
            services.AddHangfire(configuration =>
            {
                configuration.UseStorage(
                    new MySqlStorage(connectionString, new MySqlStorageOptions
                    {

                        TablesPrefix = "Hangfire"
                    }));
            });
            services.AddHangfireServer();
            services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
            return services;
        }
        public static IApplicationBuilder HangfireAppExtension(this IApplicationBuilder app, string endpoint)
        {
           app.UseHangfireDashboard(endpoint, new DashboardOptions
           {
               Authorization = new[] { new HangfireAuthorizationFilter() }
           });
            return app;
        }
    }
}
