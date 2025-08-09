using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.USSD.XML
{
    public interface IProcessResponse<TRequest, TResponse>
        where TRequest : class, IRequest
        where TResponse : class, IResponse
    {
        Task<TResponse> Process(TRequest request);
    }
    public interface IResponseMapper<TResponse, TXmlResponse>
        where TResponse : class, IResponse
        where TXmlResponse : class, IXmlResponse
    {
        Task<TXmlResponse> Map(TResponse response);
        Task<string> Process(TXmlResponse request);
    }
}
