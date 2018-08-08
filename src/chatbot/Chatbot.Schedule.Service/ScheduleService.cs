namespace Chatbot.Schedule.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Jobs;
    using Chatbot.Schedule.Service.Models;
    using Microsoft.Extensions.Hosting;
    using Quartz;

    internal class ScheduleService : BackgroundService
    {
        private readonly ISchedulerFactory schedulerFactory;

        public ScheduleService(ISchedulerFactory schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduleFunctions = GetScheduleFunctions();
            var scheduler = await schedulerFactory.GetScheduler();

            foreach (var function in scheduleFunctions)
            {
                await scheduler.ScheduleJob(CreateJob(function), CreateTrigger(function));
            }

            await scheduler.Start();
        }

        private IEnumerable<ScheduleFunction> GetScheduleFunctions()
        {
            return new List<ScheduleFunction>
            {
                new ScheduleFunction {
                    Id = 4,
                    Name = "NotifyYesterdayWinLoss",
                    GroupName = "Notification",
                    CronExpression = "0 0 16 ? * * *"
                }
            };
        }

        private static IJobDetail CreateJob(ScheduleFunction scheduleFunction)
            => JobBuilder.Create<NotificationJob>()
                .WithIdentity(scheduleFunction.Name, scheduleFunction.GroupName)
                .UsingJobData("functionId", scheduleFunction.Id)
                .Build();

        private static ITrigger CreateTrigger(ScheduleFunction scheduleFunction)
            => TriggerBuilder.Create()
                .WithIdentity($"{scheduleFunction.Name}Trigger", scheduleFunction.GroupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                //.WithCronSchedule(scheduleFunction.CronExpression)
                .Build();
    }
}