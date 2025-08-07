using Common.Libraries.Services.CQRS.Notification.PipelineBehaviors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Notification
{
    public interface INotificationDispatcher
    {
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;
    }
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IServiceProvider _provider;

        public NotificationDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }
        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        {
            var handlers = _provider.GetServices<INotificationHandler<TNotification>>();
            var behaviors = _provider.GetServices<INotificationBehavior<TNotification>>().Reverse().ToList();

            Func<Task> handlerDelegate = async () =>
            {
                foreach (var handler in handlers)
                {
                    await handler.Handle(notification, cancellationToken);
                }
            };

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;
                handlerDelegate = () => behavior.HandleAsync(notification, next, cancellationToken);
            }

            await handlerDelegate();
        }
        /*public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            var handlers = _provider.GetServices<INotificationHandler<TNotification>>();

            foreach (var handler in handlers)
            {
                await handler.Handle(notification, cancellationToken);
            }
        }*/
    }


}
