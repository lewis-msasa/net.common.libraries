using Common.Libraries.USSD.Settings;
using Common.Libraries.USSD.Sockets.Client;
using Common.Libraries.USSD.Sockets.Framing;
using Common.Libraries.USSD.XML;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Impl
{
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
    
}
