using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Notification.PipelineBehaviors
{
    public class LoggingBehavior<TNotification> : INotificationBehavior<TNotification>
     where TNotification : INotification
    {
        private readonly ILogger<LoggingBehavior<TNotification>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TNotification>> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(TNotification notification, Func<Task> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Before handling {NotificationType}", typeof(TNotification).Name);
            await next();
            _logger.LogInformation("After handling {NotificationType}", typeof(TNotification).Name);
        }
    }

}
