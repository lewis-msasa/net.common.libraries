using Common.Libraries.Services.Dtos;
using Common.Libraries.Services.EFCore.Repositories;
using Common.Libraries.Services.EFCore.UnitOfWork;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using Common.Libraries.Services.Services;
using Common.Libraries.Services.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.EFCore
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
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t))
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
        public static IServiceCollection AddEFUnitOfWorkDependencies(this IServiceCollection services, Assembly[] assemblies, Type contextType)
        {
            
            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.GetTypes();

                // Get all classes that implement IEntity
                var entityTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t))
                    .ToList();


                foreach (var entityType in entityTypes)
                {

                    var repoInterface = typeof(IUnitOfWork).MakeGenericType(entityType);
                    var repoImplementation = typeof(UnitOfWork<>).MakeGenericType(entityType, contextType);
                    services.AddScoped(repoInterface, repoImplementation);

                }

            }
            return services;
        }
        public static IServiceCollection AddEFServicesDependencies(this IServiceCollection services, Assembly[] assemblies, Type contextType)
        {
           
            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.GetTypes();

                // Get all classes that implement IEntity
                var entityTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IEntity).IsAssignableFrom(t))
                    .ToList();

                // Get all classes that extend DTO
                var dtoTypes = allTypes
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IDTO).IsAssignableFrom(t))
                    .ToDictionary(t => t.Name, t => t);

                foreach (var entityType in entityTypes)
                {


                    // Look for DTO with the same name + "Dto"
                    var dtoName = entityType.Name + "Dto";
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
