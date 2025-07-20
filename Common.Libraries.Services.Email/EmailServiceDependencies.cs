using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Email
{
    public static class EmailServiceDependencies

    {
        public static void RegisterEmailServices(this IServiceCollection services, IConfiguration Configuration, string sectionName)
        {
            var emailConfig = Configuration
               .GetSection(sectionName)
               .Get<EmailConfiguration>();
               services.AddSingleton(emailConfig);
        }
    }
}
