namespace Common.Libraries.Services.CQRS.TestApi
{
    public class CreateOrderAuditStrategy : IAuditStrategy<CreateOrderCommand>
    {
        private readonly ILogger<CreateOrderAuditStrategy> _logger;
      

        public CreateOrderAuditStrategy(ILogger<CreateOrderAuditStrategy> logger)
        {
            _logger = logger;
        }

        public Task LogStartAsync(CreateOrderCommand request)
        {
            _logger.LogInformation("[AUDIT START] User {User} is creating an order with product {Product}",
                "User1234", request.ProductId);
            return Task.CompletedTask;
        }

        public Task LogEndAsync(CreateOrderCommand request)
        {
            _logger.LogInformation("[AUDIT END] User {User} finished creating an order with product {Product}",
                "User1234", request.ProductId);
            return Task.CompletedTask;
        }
    }

}
