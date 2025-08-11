using Common.Libraries.Services.ApiRequests.Services;
using Common.Libraries.USSD.Settings;
using Flurl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Impl
{
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
                var response = await _apiRequestService.PostAsync<ServerResponse>($"{_ussdSettings.IPAddress}/",
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
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                return default!;
            }
        }
    }
}
