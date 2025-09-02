using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification.EFCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEFRepositoriesDependencies(this IServiceCollection services, Assembly[] assemblies, Type contextType)
        {
            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.GetTypes();

                // Get all classes that implement IEntity
                var entityTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ISpecEntity).IsAssignableFrom(t))
                    .ToList();


                foreach (var entityType in entityTypes)
                {
                    // Register EFRepository<TEntity, NovaIntegratorContext> as IRepository<TEntity>
                    var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
                    var repoImplementation = typeof(EFRepository<,>).MakeGenericType(entityType, contextType);
                    services.AddScoped(repoInterface, repoImplementation);

                }

            }
            return services;
        }
        public static IServiceCollection AddEntityMappers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var mapperType = typeof(IEntityMapper<,>);

            // If no assemblies passed, scan all loaded assemblies
            var assembliesToScan = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

            var mapperImplementations = assembliesToScan
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t =>
                    t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperType)
                        .Select(i => new { Implementation = t, Service = i })
                );

            foreach (var m in mapperImplementations)
            {
                services.AddScoped(m.Service, m.Implementation);
            }

            return services;
        }
        public static IServiceCollection AddEFUnitOfWorkDependencies(this IServiceCollection services, Type contextType)
        {
          
            services.AddScoped(typeof(IUnitOfWork), typeof(EfUnitOfWork<>).MakeGenericType(contextType));

            return services;
        }
        public static IServiceCollection AddEFServicesDependencies(this IServiceCollection services, Assembly[] assemblies, Type contextType, string dtoSufffix="Dto")
        {

            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.GetTypes();

                // Get all classes that implement IEntity
                var entityTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ISpecEntity).IsAssignableFrom(t))
                    .ToList();

                // Get all classes that extend DTO
                var dtoTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ISpecDto).IsAssignableFrom(t))
                    .ToDictionary(t => t.Name, t => t);

                foreach (var entityType in entityTypes)
                {


                    // Look for DTO with the same name + "Dto"
                    var dtoName = entityType.Name + dtoSufffix;
                    if (dtoTypes.TryGetValue(dtoName, out var dtoType))
                    {
                        var serviceInterface = typeof(IService<,>).MakeGenericType(entityType, dtoType);
                        var serviceImplementation = typeof(Service<,>).MakeGenericType(entityType, dtoType);
                        services.AddScoped(serviceInterface, serviceImplementation);
                    }
                }

            }
            return services;
        }
    }
}
