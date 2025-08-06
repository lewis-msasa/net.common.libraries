using Common.Libraries.Services.BackgroundWork;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Background.Quartz
{

    //public class QuartzBackgroundJobService : IBackgroundJobService
    //{
    //    private readonly IScheduler _scheduler;

    //    public QuartzBackgroundJobService(IScheduler scheduler)
    //    {
    //        _scheduler = scheduler;
    //    }

    //    public async Task Enqueue<TJob>(object data = null) where TJob : IJob
    //    {
    //        var job = CreateJob<TJob>(data);
    //        var trigger = TriggerBuilder.Create()
    //            .StartNow()
    //            .Build();

    //        await _scheduler.ScheduleJob(job, trigger);
    //    }

    //    public async Task Schedule<TJob>(TimeSpan delay, object data = null) where TJob : IJob
    //    {
    //        var job = CreateJob<TJob>(data);
    //        var trigger = TriggerBuilder.Create()
    //            .StartAt(DateBuilder.FutureDate((int)delay.TotalSeconds, IntervalUnit.Second))
    //            .Build();

    //        await _scheduler.ScheduleJob(job, trigger);
    //    }

    //    public async Task AddOrUpdateRecurring<TJob>(string jobId, string cronExpression, object data = null) where TJob : IJob
    //    {
    //        var job = CreateJob<TJob>(data, jobId);
    //        var trigger = TriggerBuilder.Create()
    //            .WithIdentity($"{jobId}-trigger")
    //            .WithCronSchedule(cronExpression)
    //            .Build();

    //        await _scheduler.ScheduleJob(job, trigger);
    //    }

    //    public async Task RemoveRecurring(string jobId)
    //    {
    //        var jobKey = new JobKey(jobId);
    //        await _scheduler.DeleteJob(jobKey);
    //    }

    //    private IJobDetail CreateJob<TJob>(object data = null, string jobId = null) where TJob : IJob
    //    {
    //        var jobBuilder = JobBuilder.Create<TJob>()
    //            .WithIdentity(jobId ?? Guid.NewGuid().ToString());

    //        if (data != null)
    //        {
    //            jobBuilder = jobBuilder.UsingJobData(new JobDataMap(data.ToDictionary()));
    //        }

    //        return jobBuilder.Build();
    //    }
    //}

}
