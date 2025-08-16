using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.BackgroundWork.Hangfire
{
    public class HangfireBackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireBackgroundJobService(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public void Enqueue<T>(Expression<Func<T, Task>> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }

        public void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        {
            BackgroundJob.Schedule(methodCall, delay);
        }

        public void AddOrUpdateRecurring<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression)
        {
            
            _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
        }

        public void RemoveRecurring(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
