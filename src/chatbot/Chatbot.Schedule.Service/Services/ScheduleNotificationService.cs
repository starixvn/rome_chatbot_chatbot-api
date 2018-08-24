namespace Chatbot.Schedule.Service
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Constants;
    using Chatbot.Schedule.Service.Jobs;
    using Chatbot.Schedule.Service.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Quartz;

    public class ScheduleNotificationService : BackgroundService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfiguration configuration;
        private readonly IJobListener jobFailureHandler;
        private readonly ILogger<ScheduleNotificationService> logger;

        public ScheduleNotificationService(
            ISchedulerFactory schedulerFactory,
            IConfiguration configuration,
            IJobListener jobFailureHandler,
            ILogger<ScheduleNotificationService> logger)
        {
            this.schedulerFactory = schedulerFactory;
            this.configuration = configuration;
            this.jobFailureHandler = jobFailureHandler;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Start execution");

            var scheduleFunctions = configuration
                .GetSection(AppSettingKey.ScheduleFunctions)
                .Get<ScheduleFunction[]>()
                .Where(f => f.GroupName == FunctionType.Notification && f.IsActive);

            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.ListenerManager.AddJobListener(jobFailureHandler);

            foreach (var function in scheduleFunctions)
            {
                logger.LogInformation($"Start schedule function {function.Name}");

                await scheduler.ScheduleJob(CreateJob(function), CreateTrigger(function));
            }

            await scheduler.Start();
        }

        private static IJobDetail CreateJob(ScheduleFunction scheduleFunction)
            => JobBuilder.Create<NotificationJob>()
                .WithIdentity(scheduleFunction.Name, scheduleFunction.GroupName)
                .UsingJobData(JobDataKey.FunctionId, scheduleFunction.Id)
                .Build();

        private static ITrigger CreateTrigger(ScheduleFunction scheduleFunction)
            => TriggerBuilder.Create()
                .WithIdentity($"{scheduleFunction.Name}Trigger", scheduleFunction.GroupName)
                .WithCronSchedule(scheduleFunction.CronExpression)
                .Build();
    }
}