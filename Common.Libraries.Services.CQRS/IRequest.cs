using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS
{
    public interface IRequest<TResponse> { }

    public interface ICommand<TResponse> : IRequest<TResponse> { }

    public interface IQuery<TResponse> : IRequest<TResponse> { }


    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

}
