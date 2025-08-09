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
    [XmlRoot("ussd")]
    public class Ussd : IxmlEntity
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("msg")]
        public string Msg { get; set; }

        [XmlElement("premium")]
        public Premium Premium { get; set; }
    }

    public class Premium
    {
        [XmlElement("cost")]
        public string Cost { get; set; }

        [XmlElement("ref")]
        public string Ref { get; set; }
    }
  
    public class  ProcessRequest
    {
        private readonly IXmlReader<Ussd> _xmlReader;

        public ProcessRequest(IXmlReader<Ussd> xmlReader)
        {
            _xmlReader = xmlReader;
        }
        public Ussd? Process(string rawRequest)
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
            if (string.IsNullOrWhiteSpace(rawRequest))
            {
                return string.Empty;
            }
            var regext = new System.Text.RegularExpressions.Regex(@"<(.|\n)*>");
            rawRequest = rawRequest.Trim();
            var match = regext.Match(rawRequest);
            return match.Success ? match.Value : string.Empty;
        }
    }

   
}
