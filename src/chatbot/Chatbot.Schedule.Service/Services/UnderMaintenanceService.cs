namespace Chatbot.Schedule.Service
{
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Constants;
    using Chatbot.Schedule.Service.Jobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Quartz;

    public class UnderMaintenanceService : BackgroundService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfiguration configuration;
        private readonly IJobListener jobFailureHandler;
        private readonly ILogger<UnderMaintenanceService> logger;

        public UnderMaintenanceService(
            ISchedulerFactory schedulerFactory,
            IConfiguration configuration,
            IJobListener jobFailureHandler,
            ILogger<UnderMaintenanceService> logger)
        {
            this.schedulerFactory = schedulerFactory;
            this.configuration = configuration;
            this.jobFailureHandler = jobFailureHandler;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (configuration.GetValue<bool>("UnderMaintenance:IsActive"))
            {
                logger.LogInformation("Start UM Schedule Service");

                var scheduler = await schedulerFactory.GetScheduler();
                scheduler.ListenerManager.AddJobListener(jobFailureHandler);

                var cronExpression = configuration.GetValue<string>("UnderMaintenance:CronExpression");

                await scheduler.ScheduleJob(CreateJob(cronExpression), CreateTrigger(cronExpression));

                await scheduler.Start();
            }
        }

        private static IJobDetail CreateJob(string cronExpression)
            => JobBuilder.Create<UnderMaintenanceJob>()
                .WithIdentity("UnderMaintenance", "UM")
                .UsingJobData(JobDataKey.CronSchedule, cronExpression)
                .Build();

        private static ITrigger CreateTrigger(string cronExpression)
            => TriggerBuilder.Create()
                .WithIdentity($"UnderMaintenanceTrigger", "UM")
                .WithCronSchedule(cronExpression)
                .Build();
    }
}