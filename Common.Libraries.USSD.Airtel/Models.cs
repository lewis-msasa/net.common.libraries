using Common.Libraries.USSD.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.Libraries.USSD.Airtel
{
    [XmlRoot("methodResponse")]
    public class UssdResponse : IXmlResponse,IxmlEntity
    {
        [XmlArray("params")]
        [XmlArrayItem("param")]
        public List<Param> Params { get; set; }
    }


    [XmlRoot("methodCall")]
    public class UssdRequest : IXmlRequest, IxmlEntity
    {
        [XmlArray("params")]
        [XmlArrayItem("param")]
        public List<Param> Params { get; set; }
    }

    public class Param
    {
        public Value Value { get; set; }
    }

    public class Value
    {
        public Struct Struct { get; set; }
    }

    public class Struct
    {
        [XmlElement("member")]
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        [XmlElement("name")]
        public string Name { get; set; }

        public MemberValue Value { get; set; }
    }

    public class MemberValue
    {
        [XmlElement("string")]
        public string String { get; set; }
    }

    public class ServerRequest : IRequest
    {
        public bool FirstRequest { get; set; }
        public string? SessionID { get; set; }
        public string? MobileNumber { get; set; }
        public string? USSDBody { get; set; }
        public string? ScreenID { get; set; }
        //public string? ServiceCode { get; set; }
        public string? ServiceProvider { get; set; }

    }
    public class ServerResponse : IResponse
    {
        public string? SessionID { get; set; }
        public string? Sequence { get; set; }
        public string? USSDBody { get; set; }
        public string? RequestType { get; set; }
        public bool ResponseRequired { get; set; } //True or False
    }
   
}
