using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD
{
    public interface IRequest{}
    public interface IXmlRequest { }

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
}
