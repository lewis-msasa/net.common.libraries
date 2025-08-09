using Common.Libraries.USSD.Settings;
using Common.Libraries.USSD.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Libraries.USSD.TNM 
{
    
   public class  ProcessRequest : IProcessRequest<UssdRequest>, IRequestMapper<UssdRequest, ServerRequest>
    {
        private readonly IXmlReader<UssdRequest> _xmlReader;
        private readonly IUSSDSettings _settings;

        public ProcessRequest(IXmlReader<UssdRequest> xmlReader, IUSSDSettings settings)
        {
            _xmlReader = xmlReader;
            _settings = settings;
        }

        public Task<ServerRequest> Map(UssdRequest request)
        {
            return Task.Run(() =>
            {
               return  new ServerRequest
                {
                    FirstRequest = request.Msg.Contains(_settings.ShortCode) && request.Type == "1" ? true ? false,
                    ServiceProvider = _settings.Network,
                    MobileNumber = request.Msisdn,
                    ScreenID = "1",
                    SessionID = request.SessionId,
                    USSDBody = request.Msg
                };
            });
        }

        public UssdRequest? Process(string rawRequest)
        {
            if (string.IsNullOrWhiteSpace(rawRequest))
            {
                return null;
            }
            var xmlPart = ProcessUssdRequestReturnXmlPart(rawRequest);
            if (string.IsNullOrWhiteSpace(xmlPart))
            {
                return null;
            }
            return _xmlReader.DeserializeXml(xmlPart);
        }
        private string ProcessUssdRequestReturnXmlPart(string rawRequest)
        {
            rawRequest = rawRequest.Trim();
            if (string.IsNullOrWhiteSpace(rawRequest))
            {
                return string.Empty;
            }
            var regext = new System.Text.RegularExpressions.Regex(@"<(.|\n)*>");
            var match = regext.Match(rawRequest);
            return match.Success ? match.Value : string.Empty;
        }
    }

  


}
