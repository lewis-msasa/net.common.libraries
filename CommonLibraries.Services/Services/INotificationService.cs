using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Services
{
    public interface IMessage
    {
        public string Message { get; }
    }
    public interface INotificationService<T> where T : IMessage
    {
        public Task<bool> SendNotification(T message); 
    }
}
