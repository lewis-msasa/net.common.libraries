using Common.Libraries.Services.ApiRequests.Services;
using Common.Libraries.USSD.Settings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Libraries.USSD
{
    public interface IApiService<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : class, IResponse
    {
        Task<TResponse> Call(TRequest request);
    }
    public class TcpCall : IApiService<ServerRequest, ServerResponse>
    {

        private IUSSDSettings _settings;
        private readonly ILogger<TcpCall> _logger;

        public TcpCall(IUSSDSettings settings, ILogger<TcpCall> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<ServerResponse> Call(ServerRequest request)
        {
            try
            {
                using (var client = new TcpClient(_settings.IPAddress, _settings.Port))
                using (var stream = client.GetStream())
                {
                    var message = JsonSerializer.Serialize(request);
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    await Task.Run(() => stream.Write(data, 0, data.Length));


                    data = new byte[2000];
                    int bytes = await Task.Run(() => stream.Read(data, 0, data.Length));
                    string responseData = Encoding.ASCII.GetString(data, 0, bytes);

                    return JsonSerializer.Deserialize<ServerResponse>(responseData) ?? default!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return default!;
            }
        }
    }


    public class ApiCall : IApiService<ServerRequest, ServerResponse>
    {
        private readonly IApiRequestService _apiRequestService;
        private readonly IUSSDSettings _ussdSettings;
        private readonly ILogger<ApiCall> _logger;

        public ApiCall(IApiRequestService apiRequestService, IUSSDSettings ussdSettings, ILogger<ApiCall> logger)
        {
            _apiRequestService = apiRequestService;
            _ussdSettings = ussdSettings;
            _logger = logger;
        }

        public async Task<ServerResponse> Call(ServerRequest request)
        {
            try
            {
                var response = await _apiRequestService.PostAsync<ServerResponse>($"{_ussdSettings.ApiUrl}",
                          new Dictionary<string, string> {
                       { "Content-Type", "application/json" },
                       { "Authorization", $"Bearer " }
                           },
                           request,
                           async (req, resp, code) =>
                           {
                               await Task.Run(() =>
                               {
                                   _logger.LogInformation(req);
                               });

                           }
                       );
                return response.result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return default!;
            }
        }
    }
}
