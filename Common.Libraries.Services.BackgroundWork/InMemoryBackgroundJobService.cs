using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.BackgroundWork
{
    public class InMemoryBackgroundJobService : IWithEnqueue, IWithSchedule
    {
        public void Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            var compiled = methodCall.Compile();
            var instance = Activator.CreateInstance<T>();
            _ = Task.Run(() => compiled(instance));
        }

        public void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        {
            var compiled = methodCall.Compile();
            var instance = Activator.CreateInstance<T>();
            _ = Task.Run(async () =>
            {
                await Task.Delay(delay);
                await compiled(instance);
            });
        }
    }
}
