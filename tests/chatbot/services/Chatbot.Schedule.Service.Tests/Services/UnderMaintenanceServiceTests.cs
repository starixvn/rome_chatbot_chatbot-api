namespace Chatbot.Schedule.Service.Tests.Services
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class UnderMaintenanceServiceTests
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IConfiguration configuration;
        private readonly IJobListener jobFailureHandler;
        private readonly ILogger<UnderMaintenanceService> logger;
        private readonly UnderMaintenanceService underMaintenanceService;

        public UnderMaintenanceServiceTests()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json");

            configuration = builder.Build();

            schedulerFactory = Substitute.For<ISchedulerFactory>();
            jobFailureHandler = Substitute.For<IJobListener>();
            logger = Substitute.For<ILogger<UnderMaintenanceService>>();
            underMaintenanceService = new UnderMaintenanceService(
                schedulerFactory,
                configuration,
                jobFailureHandler,
                logger);
        }

        [Fact]
        public async Task ExecuteAsync_Always_AddJobListener()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await underMaintenanceService.StartAsync(new CancellationToken());

            // Assert
            scheduler.ListenerManager.Received().AddJobListener(jobFailureHandler);
        }

        [Fact]
        public async Task ExecuteAsync_Always_UnderMaintenaceNotification()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await underMaintenanceService.StartAsync(new CancellationToken());

            // Assert
            await scheduler.Received(1).ScheduleJob(
                Arg.Is<IJobDetail>(j =>
                    j.Key.Equals(JobKey.Create("UnderMaintenance", "UM"))),
                Arg.Is<ITrigger>(t =>
                    t.Key.Equals(new TriggerKey("UnderMaintenanceTrigger", "UM"))));
        }

        [Fact]
        public async Task ExecuteAsync_Always_StartScheduler()
        {
            // Arrange
            var scheduler = await schedulerFactory.GetScheduler();

            // Act
            await underMaintenanceService.StartAsync(new CancellationToken());

            // Assert
            await scheduler.Received().Start();
        }
    }
}