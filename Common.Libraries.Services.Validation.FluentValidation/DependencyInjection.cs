using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Validation.FluentValidation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFluentValidator(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach(var assembly in assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
            services.AddScoped(typeof(IRequestValidator<>), typeof(FluentRequestValidator<>));
            return services;
        }
    }
}
