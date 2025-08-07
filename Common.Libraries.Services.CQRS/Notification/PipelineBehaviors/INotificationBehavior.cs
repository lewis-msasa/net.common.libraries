using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Notification.PipelineBehaviors
{
    public interface INotificationBehavior<TNotification> where TNotification : INotification
    {
        Task HandleAsync(TNotification notification, Func<Task> next, CancellationToken cancellationToken);
    }

}
