using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.CQRS.Notification
{
    public interface INotificationHandler<in TNotification>
     where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }

}
