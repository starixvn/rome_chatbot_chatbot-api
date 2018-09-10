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

        //[Fact]
        //public async Task Execute_Always_SendMessageForEachUser()
        //{
        //    // Arrange
        //    var context = Substitute.For<IJobExecutionContext>();
        //    IDictionary<string, object> keyValuePairs = new Dictionary<string, object> { { "functionId", 4 } };
        //    context.JobDetail.JobDataMap.Returns(new JobDataMap(keyValuePairs));
        //    messageService.GetFunctionMessages(4, 0).Returns(
        //        new List<User> {
        //            new User { CustId = 1, Id = 2 },
        //            new User { CustId = 2, Id = 3 }
        //        });

        //    // Act
        //    await notificationJob.Execute(context);

        //    // Assert
        //    await messageSender.Received().SendMessageToUser(2, "message");
        //    await messageSender.Received().SendMessageToUser(3, "message");
        //    await messageSender.Received(2).SendMessageToUser(Arg.Any<int>(), Arg.Any<int>());
        //}
    }
}