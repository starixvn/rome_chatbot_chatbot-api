namespace Chatbot.Schedule.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Jobs;
    using Chatbot.Schedule.Service.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Quartz;

    internal class ScheduleNotificationService : BackgroundService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfiguration configuration;
        private readonly IJobListener jobFailureHandler;

        public ScheduleNotificationService(
            ISchedulerFactory schedulerFactory,
            IConfiguration configuration,
            IJobListener jobFailureHandler)
        {
            this.schedulerFactory = schedulerFactory;
            this.configuration = configuration;
            this.jobFailureHandler = jobFailureHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduleFunctions = configuration.GetSection("ScheduleFunctions").Get<ScheduleFunction[]>();
            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.ListenerManager.AddJobListener(jobFailureHandler);

            foreach (var function in scheduleFunctions)
            {
                await scheduler.ScheduleJob(CreateJob(function), CreateTrigger(function));
            }

            await scheduler.Start();
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
                .WithCronSchedule(scheduleFunction.CronExpression)
                .Build();
    }
}