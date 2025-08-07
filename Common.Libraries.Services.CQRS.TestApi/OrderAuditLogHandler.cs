using Common.Libraries.Services.CQRS.Notification;

namespace Common.Libraries.Services.CQRS.TestApi
{
    public class OrderAuditLogHandler : INotificationHandler<OrderCreatedNotification>
    {
        public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"User {notification.OrderId} created — logging to audit system.");
            return Task.CompletedTask;
        }
    }

}
