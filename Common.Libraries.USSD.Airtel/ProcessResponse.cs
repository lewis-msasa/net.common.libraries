using Common.Libraries.USSD.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.Airtel
{
    public class ProcessResponse : IProcessResponse<ServerRequest, ServerResponse>, IResponseMapper<ServerResponse, UssdResponse>
    {
        private readonly IXmlReader<UssdResponse> _xmlReader;

        public Task<UssdResponse> Map(ServerResponse response)
        {
            //Convert server response to UssdResponse object
            return Task.Run(() => MapToUssdResponse(response));
        }
        public Task<ServerResponse> Process(ServerRequest request)
        {
            //1. do api call here
            throw new NotImplementedException();
        }

        public async Task<string> Process(UssdResponse response)
        {
            //Serialize UssdResponse object to XML string
            //Return the XML string
           var xml = await Task.Run(() => _xmlReader.SerializeXml(response));
            return PrependHeaders(xml);
        }
        private UssdResponse MapToUssdResponse(ServerResponse response)
        {

            return new UssdResponse
            {
                Params = new List<Param>
                    {
                        new Param
                        {
                            Value = new Value
                            {
                                Struct = new Struct
                                {
                                    Members = new List<Member>
                                    {
                                        new Member { Name = "SESSION_ID", Value = new MemberValue { String = response.SessionID } },
                                        new Member { Name = "SEQUENCE", Value = new MemberValue { String = response.Sequence } },
                                        new Member { Name = "USSD_BODY", Value = new MemberValue { String = response.USSDBody } },
                                        new Member { Name = "REQUEST_TYPE", Value = new MemberValue { String = response.RequestType } },
                                        new Member { Name = "END_OF_SESSION", Value = new MemberValue { String = response.ResponseRequired ? "True" : "False" } },  //True or False
                                    }
                                }
                            }
                        }
                    }
            };

        }
        private string PrependHeaders(string theXML)
        {
            var headers = new System.Text.StringBuilder();
            headers.AppendLine("HTTP/1.1 200 OK");
            headers.AppendLine("Date: " + DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"));
            headers.AppendLine("Server: Apache/2.2.17 (Win32) mod_ssl/2.2.17 OpenSSL/0.9.8o PHP/5.3.4 mod_perl/2.0.4 Perl/v5.10.1");
            headers.AppendLine("X-Powered-By: PHP/5.3.5");
            headers.AppendLine("Content-Length: " + theXML.Length);
            headers.AppendLine("Keep-Alive: timeout=45000, max=50000");
            headers.AppendLine("Connection: Keep-Alive");
            headers.AppendLine("Content-Type: text/xml");
            headers.AppendLine();
            headers.Append(theXML);
            return headers.ToString();
        }
    }
}
