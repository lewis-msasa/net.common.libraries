using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Flurl.Utils
{
    public class UntrustedClient
    {
        public UntrustedClient() {
          
        }
        public void GetHandler()
        {
            FlurlHttp.Clients.WithDefaults(builder =>
                      builder.ConfigureInnerHandler(handler =>
                      {
                          if (handler is HttpClientHandler h)
                              h.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                         
                      }));
        }
    }
}
