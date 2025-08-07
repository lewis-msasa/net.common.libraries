using Common.Libraries.Services.CQRS.Notification;
using Common.Libraries.Services.CQRS.Notification.PipelineBehaviors;
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

    public static class ServiceProviderExtensions
    {
        public static bool TryGetService<T>(this IServiceProvider provider, out T instance)
        {
            instance = provider.GetService<T>();
            return instance != null;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            assemblies = assemblies.Append(typeof(INotificationDispatcher).Assembly).ToArray();
            var handlerInterfaceType = typeof(INotificationHandler<>);

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly
                    .GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface)
                    .SelectMany(t => t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
                        .Select(i => new { HandlerType = t, InterfaceType = i }));

                foreach (var type in handlerTypes)
                {
                    services.AddScoped(type.InterfaceType, type.HandlerType);
                }
            }
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

            return services;
        }
        public static IServiceCollection AddNotificationPipelines(this IServiceCollection services, Assembly[] assemblies = default!)
        {
            assemblies = assemblies.Append(typeof(INotificationDispatcher).Assembly).ToArray();
         

            var behaviorType = typeof(INotificationBehavior<>);

            var openGenericBehaviors = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.IsGenericTypeDefinition &&
                    t.GetInterfaces()
                        .Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == behaviorType))
                .ToList();

            foreach (var behavior in openGenericBehaviors)
            {
                services.AddScoped(behaviorType, behavior);
            }

            return services;
            
        }
        public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            assemblies = assemblies.Append(typeof(IDispatcher).Assembly).ToArray();
            var handlerInterface = typeof(IRequestHandler<,>);
            var voidHandlerInterface = typeof(IRequestHandler<>);

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
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == voidHandlerInterface)
                        {
                            services.AddTransient(iface, type);
                        }
                    }
                }
            }

            services.AddScoped<IDispatcher, Dispatcher>();
            return services;
        }
        //public static IServiceCollection RegisterCORSBehaviorsServices(this IServiceCollection services)
        //{
        //    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        //    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        //    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

        //    return services;

        //}

        public static IServiceCollection AddPipelines(this IServiceCollection services, Assembly[] assemblies = default!)
        {
            assemblies = assemblies.Append(typeof(IDispatcher).Assembly).ToArray();
            var behaviorType = typeof(IPipelineBehavior<,>);

            var openGenericBehaviors = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.IsGenericTypeDefinition &&
                    t.GetInterfaces()
                        .Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == behaviorType))
                .ToList();

            foreach (var behavior in openGenericBehaviors)
            {
                services.AddScoped(behaviorType, behavior);
            }

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
