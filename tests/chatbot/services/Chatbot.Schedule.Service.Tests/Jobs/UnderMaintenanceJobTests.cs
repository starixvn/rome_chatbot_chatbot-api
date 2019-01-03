namespace Chatbot.Schedule.Service.Tests.Jobs
{
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Jobs;
    using NSubstitute;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Quartz;
    using Xunit;

    public class UnderMaintenanceJobTests
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly MessageService messageService;
        private readonly UnderMaintenanceJob underMaintenanceJob;

        public UnderMaintenanceJobTests()
        {
            maintenanceService = Substitute.For<IMaintenanceService>();
            messageService = Substitute.For<MessageService>();
            underMaintenanceJob = new UnderMaintenanceJob(maintenanceService, messageService);
        }

        [Fact]
        public void Execute_Always_CallsGetUnderMaintenanceTime()
        {
            // Arrange
            var context = Substitute.For<IJobExecutionContext>();
            GlobalVariables.IsUM = true;
            var underMaintenanceTime = new UnderMaintenanceTime(false, string.Empty, string.Empty);
            maintenanceService.GetUnderMaintenanceTime().Returns(Task.FromResult(underMaintenanceTime));

            // Act
            underMaintenanceJob.Execute(context).ConfigureAwait(false);

            // Assert
            maintenanceService.Received().GetUnderMaintenanceTime();
        }
    }
}