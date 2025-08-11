using Common.Libraries.USSD.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.TNM
{
    public  class ProcessResponse : IProcessResponse<ServerRequest, ServerResponse>, IResponseMapper<ServerResponse, UssdResponse>
    {
        private readonly IXmlReader<UssdResponse> _xmlReader;
        private readonly IApiService<ServerRequest, ServerResponse> _apiService;

        public ProcessResponse(IXmlReader<UssdResponse> xmlReader, IApiService<ServerRequest, ServerResponse> apiService)
        {
            _xmlReader = xmlReader;
            _apiService = apiService;
        }

        public Task<UssdResponse> Map(ServerResponse response)
        {
            return Task.Run(() => new UssdResponse
            {
                Msg = response.USSDBody,
                Type = response.ResponseRequired ? "2" : "3",
                Premium = new Premium
                {
                    Cost = "",
                    Ref = ""
                }
            });
        }

        public Task<ServerResponse> Process(ServerRequest request)
        {
            //api call here
            return _apiService.Call(request);
        }

        public async Task<string> Process(UssdResponse response)
        {
            var xml = await Task.Run(() => _xmlReader.SerializeXml(response));
            return PrependHeaders(xml);
        }
        private string PrependHeaders(string theXML)
        {
            var headers = new System.Text.StringBuilder();
            headers.AppendLine("HTTP/1.1 200 OK");
            headers.AppendLine("Date: " + DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"));
            headers.AppendLine("Server: Apache/2.2.17 (Win32) mod_ssl/2.2.17 OpenSSL/0.9.8o PHP/5.3.4 mod_perl/2.0.4 Perl/v5.10.1");
            headers.AppendLine("X-Powered-By: PHP/5.3.5");
            headers.AppendLine("Content-Length: " + theXML.Length);
            headers.AppendLine("Keep-Alive: timeout=45000, max=50000");
            headers.AppendLine("Connection: Keep-Alive");
            headers.AppendLine("Content-Type: text/xml");
            headers.AppendLine();
            headers.Append(theXML);
            return headers.ToString();
        }
    }
}
