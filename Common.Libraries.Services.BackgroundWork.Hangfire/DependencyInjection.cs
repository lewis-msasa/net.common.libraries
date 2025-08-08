using Hangfire;
using Hangfire.MemoryStorage;
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
        public static IServiceCollection AddHangfireDependencies(this IServiceCollection services, IConfiguration Configuration,bool inMemory = false)
        {
            
            services.AddHangfire(configuration =>
            {
                if (!inMemory)
                {
                    var connectionString = Configuration.GetConnectionString("HangfireConnection");
                    configuration.UseStorage(
                        new MySqlStorage(connectionString, new MySqlStorageOptions
                        {

                            TablesPrefix = "Hangfire"
                        }));
                }
                else
                {
                    configuration.UseMemoryStorage();
                }
            });
            services.AddHangfireServer();
           
            services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
            return services;
        }
        public static IApplicationBuilder HangfireAppExtension(this IApplicationBuilder app,string username,string password, string endpoint)
        {
           app.UseHangfireDashboard(endpoint, new DashboardOptions
           {
               Authorization = [new HangfireBasicAuthFilter(username, password)]
           });
            return app;
        }
    }
}
