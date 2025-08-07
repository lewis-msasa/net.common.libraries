using Common.Libraries.Services.CQRS.Notification;

namespace Common.Libraries.Services.CQRS.TestApi
{
    public class SendOrderEmailHandler : INotificationHandler<OrderCreatedNotification>
    {
        public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Sending welcome email to {notification.Email}");
            return Task.CompletedTask;
        }
    }

}
