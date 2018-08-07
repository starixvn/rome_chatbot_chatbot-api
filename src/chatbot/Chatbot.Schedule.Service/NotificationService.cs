namespace Chatbot.Schedule.Service
{
    using System.Collections.Specialized;
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Jobs;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Impl;

    internal class NotificationService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });
            var scheduler = await factory.GetScheduler();

            var job = JobBuilder.Create<GetCustomerYesterdayWinLossReportSettingJob>()
                .WithIdentity("YesterdayWinLossJob", "YesterdayWinLoss")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("YesterdayWinLossTrigger", "YesterdayWinLoss")
                .StartNow()
                //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(16, 00))
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}