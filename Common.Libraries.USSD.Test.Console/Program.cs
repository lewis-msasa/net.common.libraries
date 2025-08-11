// See https://aka.ms/new-console-template for more information


using Common.Libraries.Services.ApiRequests.Flurl.Services;
using Common.Libraries.Services.ApiRequests.Services;
using Common.Libraries.USSD;
using Common.Libraries.USSD.Airtel;
using Common.Libraries.USSD.Settings;
using Common.Libraries.USSD.Sockets.Framing;
using Common.Libraries.USSD.Sockets.Server;
using Common.Libraries.USSD.TNM;
using Common.Libraries.USSD.XML;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Runtime;
using System.Text;

using IHost host = Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
             })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();  
                });
                //services.AddUSSDAirtelDependencies([]);
                services.AddUSSDTNMDependencies([]);
                services.Configure<USSDSettings>(context.Configuration.GetSection("USSDSettings"));
                services.AddSingleton<IUSSDSettings>(sp => sp.GetRequiredService<IOptions<USSDSettings>>().Value);
                services.AddScoped<IProtocolFramer, NoFraming>();
                services.AddSingleton<IApiRequestService,FlurlApiRequestService>();
                //services.AddKeyedScoped<IApiService<ServerRequest, ServerResponse>, TcpCall>("TCP");
                services.AddScoped<IApiService<ServerRequest, ServerResponse>, ApiCall>();
                //services.AddTransient<App>(); // Register main app entry point
            })
            .Build();
var TNMSocket = host.Services.GetRequiredService<SocketTcpServer>();
var settings = host.Services.GetRequiredService<IUSSDSettings>();
await TNMSocket.StartAsync();

Console.WriteLine("Hello, World!");
