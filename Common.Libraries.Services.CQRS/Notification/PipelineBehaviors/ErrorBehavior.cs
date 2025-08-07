using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Notification.PipelineBehaviors
{
    public class ErrorHandlingBehavior<TNotification> : INotificationBehavior<TNotification>
     where TNotification : INotification
    {
        public async Task HandleAsync(TNotification notification, Func<Task> next, CancellationToken cancellationToken)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling notification {typeof(TNotification).Name}: {ex.Message}");
                throw;
            }
        }
    }

}
