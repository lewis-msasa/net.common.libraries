using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.BackgroundWork
{
    public interface IWithRecurringJobs
    {
        void AddOrUpdateRecurring<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression);
        void RemoveRecurring(string jobId);
    }
    public interface IWithEnqueue
    {
        void Enqueue<T>(Expression<Func<T, Task>> methodCall);
    }
    public interface IWithSchedule
    {
        void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
    }
    public interface IBackgroundJobService : IWithRecurringJobs, IWithEnqueue, IWithSchedule
    {
     
    }
}
