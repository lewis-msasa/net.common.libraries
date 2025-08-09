using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.XML
{
    public interface IProcessRequest<T> where T : class, IXmlRequest
    {
        T? Process(string rawRequest);
    }
    public interface IRequestMapper<TXmlRequest, TRequest>
       where TRequest : class, IRequest
       where TXmlRequest : class, IXmlRequest
    {
        Task<TRequest> Map(TXmlRequest request);
    }
}
