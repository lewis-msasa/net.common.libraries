using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD
{
    public interface IResponse{}
    public interface IXmlResponse { }

    public class ServerResponse : IResponse
    {
        public string? SessionID { get; set; }
        public string? USSDBody { get; set; }

        public string? ScreenID { get; set; }
        public bool ResponseRequired { get; set; } 
    }
}
