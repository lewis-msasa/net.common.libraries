namespace Common.Libraries.Services.CQRS.TestApi
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
