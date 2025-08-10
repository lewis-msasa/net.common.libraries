using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Settings
{
    public class USSDSettings : IUSSDSettings
    {
        public string IPAddress { get; set; } 
        public int Port { get; set; }

        public int SessionMinutesToRefresh { get; set; }

        public string Network {  get; set; }    

        public string ShortCode { get; set; }
    }
}
