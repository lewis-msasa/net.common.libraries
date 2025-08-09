using Common.Libraries.USSD.Settings;
using Common.Libraries.USSD.XML;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Common.Libraries.USSD.Airtel
{

   

    public class ProcessRequest : IProcessRequest<UssdRequest>, IRequestMapper<UssdRequest, ServerRequest>
    {
        private readonly IXmlReader<UssdRequest> _xmlReader;
        private readonly IUSSDSettings _settings;

        public ProcessRequest(IXmlReader<UssdRequest> xmlReader, IUSSDSettings settings)
        {
            _xmlReader = xmlReader;
            _settings = settings;
        }

        public async Task<ServerRequest> Map(UssdRequest request)
        {
            var dict = await Task.Run(() => request.Params[0].Value.Struct.Members
              .ToDictionary(m => m.Name, m => m.Value.String));

            var req =  new ServerRequest
            {
                
                SessionID = dict.TryGetValue("SESSION_ID", out var sid) ? sid : null,
                MobileNumber = dict.TryGetValue("MOBILE_NUMBER", out var mobile) ? mobile : null,
                USSDBody = dict.TryGetValue("USSD_BODY", out var ussd) ? ussd : null,
                ScreenID = dict.TryGetValue("SEQUENCE", out var seq) ? seq : null,
                //ServiceCode = dict.TryGetValue("SERVICE_KEY", out var svc) ? svc : null
            };
            req.FirstRequest = req.ScreenID == "1" && req.USSDBody.Contains(_settings.ShortCode) ? true : false;
            return req;
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
            var match = Regex.Match(rawRequest, @"<\?xml", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            var startIndex = match.Index;
            if(startIndex < 0)
            {
                return string.Empty;
            }
            
            return rawRequest.Substring(startIndex);
        }
    }

}
