using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS
{
    public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next);
    }

    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    public interface IVoidPipelineBehavior<in TRequest>
    where TRequest : IRequest
    {
        Task Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate next);
    }
    public delegate Task RequestHandlerDelegate();

}
