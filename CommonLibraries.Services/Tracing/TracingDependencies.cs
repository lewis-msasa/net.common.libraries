using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;


namespace Common.Libraries.Services.tracing
{
    public static class TracingDependencies
    {
        public static void RegisterTracingServices(this IServiceCollection services, IConfiguration Configuration, string serviceName, string[] sources)
        {
          

            var zipKinEndPoint = Configuration.GetValue<string>("Zipkin:Endpoint");
            var builder = services.AddOpenTelemetry().WithTracing((builder) =>
            {
                var build = builder
                 .SetResourceBuilder(ResourceBuilder.CreateDefault()
                     .AddService(serviceName))
                 .AddSource(sources)
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation();
                 //.AddRuntimeInstrumentation();
                if (zipKinEndPoint != null)
                {
                    build.AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri(zipKinEndPoint);
                    });
                }
                 build.AddConsoleExporter();



           });
        }
    }
}
