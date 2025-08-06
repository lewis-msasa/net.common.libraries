namespace Common.Libraries.Services.CQRS.TestApi
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
