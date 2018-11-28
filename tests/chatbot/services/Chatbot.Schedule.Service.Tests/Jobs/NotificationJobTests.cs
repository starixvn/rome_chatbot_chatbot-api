namespace Chatbot.Schedule.Service.Tests.Jobs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Jobs;
    using Chatbot.Schedule.Service.MessageHandler;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class NotificationJobTests
    {
        private readonly IMessageSender messageSender;
        private readonly MessageService messageService;
        private readonly IJob notificationJob;

        public NotificationJobTests()
        {
            messageSender = Substitute.For<IMessageSender>();
            messageService = Substitute.For<MessageService>();
            notificationJob = new NotificationJob(messageSender, messageService);
        }

        [Fact]
        public async Task Execute_Always_SendMessageForEachUser()
        {
            // Arrange
            var context = Substitute.For<IJobExecutionContext>();
            IDictionary<string, object> keyValuePairs = new Dictionary<string, object> { { "functionId", 4 } };
            context.JobDetail.JobDataMap.Returns(new JobDataMap(keyValuePairs));
            messageService.GetFunctionMessages(4).Returns(
                new List<FunctionMessage> {
                   new FunctionMessage {Message = "Message", UserId = 1, SelfdestructionPeriod=30},
                   new FunctionMessage {Message = "Message1", UserId = 2, SelfdestructionPeriod=30}
                });

            // Act
            await notificationJob.Execute(context);

            // Assert
            await messageSender.Received().SendMessageToUser(1, 4, "Message", 30);
            await messageSender.Received().SendMessageToUser(2, 4, "Message1", 30);
            await messageSender.Received(2).SendMessageToUser(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>());
        }
    }
}