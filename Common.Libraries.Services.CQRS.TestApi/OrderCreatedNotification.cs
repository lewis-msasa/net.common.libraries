using Common.Libraries.Services.CQRS.Notification;

namespace Common.Libraries.Services.CQRS.TestApi
{
    public class OrderCreatedNotification : INotification
    {
        public Guid OrderId { get; }
        public string Email { get; }

        public OrderCreatedNotification(Guid OrderId, string email)
        {
            this.OrderId = OrderId;
            Email = email;
        }
    }

}
