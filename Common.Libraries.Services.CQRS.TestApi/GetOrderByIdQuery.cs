namespace Common.Libraries.Services.CQRS.TestApi
{
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public Guid OrderId { get; set; }

        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
