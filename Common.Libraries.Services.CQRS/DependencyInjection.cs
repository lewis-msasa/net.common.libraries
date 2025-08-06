using Common.Libraries.Services.CQRS.PipelineBehaviors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            assemblies = assemblies.Append(typeof(IDispatcher).Assembly).ToArray();
            var handlerInterface = typeof(IRequestHandler<,>);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == handlerInterface)
                        {
                            services.AddTransient(iface, type);
                        }
                    }
                }
            }

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }
        public static IServiceCollection RegisterCORSBehaviorsServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

            return services;

        }

        public static IServiceCollection AddPipelines(this IServiceCollection services, Assembly[] assemblies = default!)
        {
            assemblies = assemblies.Append(typeof(IDispatcher).Assembly).ToArray();
            var behaviorInterface = typeof(IPipelineBehavior<,>);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                       
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == behaviorInterface)
                        {
                            services.AddTransient(iface, type);
                        }
                    }
                }
            }
            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }


        public static IServiceCollection AddRequestHandlersAndPipelines(this IServiceCollection services, Assembly[] assemblies)
        {
            assemblies = assemblies.Append(typeof(IDispatcher).Assembly).ToArray();
            var handlerInterface = typeof(IRequestHandler<,>);
            var behaviorInterface = typeof(IPipelineBehavior<,>);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == handlerInterface)
                        {
                            services.AddTransient(iface, type);
                        }

                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == behaviorInterface)
                        {
                            services.AddTransient(iface, type);
                        }
                    }
                }
            }

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }
    }

}
