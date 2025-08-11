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
        [XmlElement("value")]
        public Value Value { get; set; }
    }

    public class Value
    {
        [XmlElement("struct")]
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
        [XmlElement("value")]
        public MemberValue Value { get; set; }
    }

    public class MemberValue
    {
        [XmlElement("string")]
        public string String { get; set; }
    }

   
   
   
}
