namespace Common.Libraries.Services.CQRS.TestApi
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(ILogger<CreateOrderHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // your business logic here
            _logger.LogInformation("Creating order for product {ProductId} with quantity {Quantity}",
                request.ProductId, request.Quantity);
            return Guid.NewGuid();
        }
    }
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly ILogger<GetOrderByIdHandler> _logger;
        public GetOrderByIdHandler(ILogger<GetOrderByIdHandler> logger)
        {
            _logger = logger;
        }
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            // your business logic here
            _logger.LogInformation("Retrieving order with ID {OrderId}", request.OrderId);
            return new OrderDto { Id = request.OrderId, ProductId = "ABC", Quantity = 3 };
        }
    }
}
