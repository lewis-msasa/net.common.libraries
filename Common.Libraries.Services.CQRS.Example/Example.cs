using Common.Libraries.Services.CQRS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Example
{
   
    //public class CreateOrderCommand : ICommand<Guid>
    //{
    //    public string ProductId { get; set; } = default!;
    //    public int Quantity { get; set; }
    //}
    //public class GetOrderByIdQuery : IQuery<OrderDto>
    //{
    //    public Guid OrderId { get; set; }

    //    public GetOrderByIdQuery(Guid orderId)
    //    {
    //        OrderId = orderId;
    //    }
    //}
    //public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    //{
    //    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    //    {
    //        // your business logic here
    //        return Guid.NewGuid();
    //    }
    //}

    //public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    //{
    //    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    //    {
    //        // your business logic here
    //        return new OrderDto { Id = request.OrderId, ProductId = "ABC", Quantity = 3 };
    //    }
    //}
    //builder.Services.AddScoped<IDispatcher, Dispatcher>();

    //builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, Guid>, CreateOrderHandler>();
    //builder.Services.AddScoped<IRequestHandler<GetOrderByIdQuery, OrderDto>, GetOrderByIdHandler>();

    //app.MapPost("/orders", async(CreateOrderCommand command, IDispatcher dispatcher) =>
    //{
    //    var orderId = await dispatcher.Send(command);
    //    return Results.Ok(orderId);
    //});

    //app.MapGet("/orders/{id:guid}", async (Guid id, IDispatcher dispatcher) =>
    //{
    //    var query = new GetOrderByIdQuery(id);
    //    var order = await dispatcher.Send(query);
    //    return Results.Ok(order);
    //});




    //public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    //{
    //    var requestType = request.GetType();
    //    var responseType = typeof(TResponse);

    //    // Detect if it's a command or query using marker interfaces
    //    if (request is ICommand<TResponse>)
    //    {
    //        _logger.LogInformation("Handling COMMAND: {RequestType}", requestType.Name);
    //        // Add command-specific logic here: permission checks, audit logs, etc.
    //    }
    //    else if (request is IQuery<TResponse>)
    //    {
    //        _logger.LogInformation("Handling QUERY: {RequestType}", requestType.Name);
    //        // Add query-specific logic here: caching, metrics, etc.
    //    }
    //    else
    //    {
    //        _logger.LogWarning("Unknown IRequest type: {RequestType}", requestType.Name);
    //    }

    //    var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
    //    var handler = _provider.GetService(handlerType);

    //    if (handler == null)
    //        throw new InvalidOperationException($"No handler registered for {requestType.Name}");

    //    var method = handlerType.GetMethod("Handle");

    //    var result = method!.Invoke(handler, new object[] { request, cancellationToken });
    //    return await (Task<TResponse>)result!;
    //}
}

