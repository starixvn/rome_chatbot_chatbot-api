namespace Chatbot.Schedule.Service.Tests.MessageHandler
{
    using System.Collections.Generic;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.MessageHandler;
    using NSubstitute;
    using Quartz;
    using Xunit;

    public class SendMessageByFunctionJobTests
    {
        private readonly MessageService messageService;
        private readonly IJob job;

        public SendMessageByFunctionJobTests()
        {
            messageService = Substitute.For<MessageService>();
            job = new SendMessageToUserJob(messageService);
        }

        [Fact]
        public void Execute_ResultIsNotOk_ThrowJobExecutionException()
        {
            // Arrange
            var context = Substitute.For<IJobExecutionContext>();
            IDictionary<string, object> keyValuePairs = new Dictionary<string, object> {
                { "functionId", 4 },
                { "userId", 2 },
                { "message", "Message"}
            };
            context.JobDetail.JobDataMap.Returns(new JobDataMap(keyValuePairs));
            messageService.SendMessage(2, "Message", 4).Returns(new Result { Code = 1 });

            // Act && Assert
            Assert.Throws<JobExecutionException>(() => job.Execute(context).GetAwaiter().GetResult());
        }
    }
}