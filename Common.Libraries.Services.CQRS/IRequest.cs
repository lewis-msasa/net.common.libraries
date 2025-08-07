using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS
{
    public interface IRequest { }

    public interface IRequest<out TResponse> : IRequest { }

    public interface ICommand : IRequest { }
    public interface ICommand<TResponse> : IRequest<TResponse>, ICommand { }

    public interface IQuery : IRequest { }
    public interface IQuery<TResponse> : IRequest<TResponse>, IQuery { }

    public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
    {
        Task Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

}
