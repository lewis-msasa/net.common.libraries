using Common.Libraries.USSD.Sockets.Server;
using Common.Libraries.USSD.XML;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Airtel
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUSSDAirtelDependencies(this IServiceCollection services, Assembly[] assemblies)
        {


            services.AddScoped<SocketTcpServer, AirtelSocketServer>();
            services.AddScoped(typeof(IXmlReader<>), typeof(XmlReader<>));
            services.AddTransient<IProcessRequest<UssdRequest>, ProcessRequest>();
            services.AddTransient<IRequestMapper<UssdRequest, ServerRequest>, ProcessRequest>();
            services.AddTransient<IProcessResponse<ServerRequest, ServerResponse>, ProcessResponse>();
            services.AddTransient<IResponseMapper<ServerResponse, UssdResponse>, ProcessResponse>();
            return services;
        }
    }
}
