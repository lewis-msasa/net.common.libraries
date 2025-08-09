using Common.Libraries.USSD.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.Libraries.USSD.TNM
{
    [XmlRoot("ussd")]
    public class UssdResponse : IxmlEntity, IXmlResponse
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


    [XmlRoot("ussd")]
    public class UssdRequest : IxmlEntity, IXmlRequest
    {
        [XmlElement("sessionid")]
        public string SessionId { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("msg")]
        public string Msg { get; set; }

        [XmlElement("msisdn")]
        public string Msisdn { get; set; }
    }

    public class ServerRequest : IRequest
    {
        public bool FirstRequest { get; set; }
        public string? SessionID { get; set; }
        public string? MobileNumber { get; set; }
        public string? USSDBody { get; set; }
        public string? ScreenID { get; set; }
        
        public string? ServiceProvider { get; set; }

    }
    public class ServerResponse : IResponse
    {
        public string? SessionID { get; set; }
        public string? Sequence { get; set; }
        public string? USSDBody { get; set; }
        public string? RequestType { get; set; }
        public bool ResponseRequired { get; set; }
    }
}
