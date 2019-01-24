namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageLoggers.Params.Db;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Shared.Configurations;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;
    using MessengerClient = Messenger.Client;

    [Trait("Modules", "MessageLogger")]
    public class MessageLoggerTests
    {
        private readonly IMessageLogger messageLogger;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly Func<DateTime> getNow;

        public MessageLoggerTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            getNow = Substitute.For<Func<DateTime>>();
            messageLogger = new MessageLogger(dynamicRepository, chatbotConfigs, getNow);
        }

        [Fact]
        public void GetMessageLogs_Always_ReturnMessageLogs()
        {
            // Arrange
            var userId = 4332;
            var fromDate = new DateTime(1989, 12, 12);
            var toDate = new DateTime(1989, 12, 13);
            var messengerType = 1;
            var functionId = 1;
            var stubMessages = new List<Message> { new Message(1, 2, 3, 1, "messengername", 1, "content", new DateTime(1989, 12, 13), 1) };

            getNow.Invoke().Returns(new DateTime(1989, 12, 12));

            Expression<Predicate<GetMessageLogCriteria>> isArgFunction =
                criteria => criteria.UserId == userId
                                    && criteria.MessengerType == messengerType
                                    && criteria.FunctionId == functionId
                                    && criteria.FromDate == fromDate.Date
                                    && criteria.ToDate == toDate.Date.AddDays(1).AddSeconds(-1);

            dynamicRepository.FetchAsync<Message>(Arg.Is(isArgFunction))
                .Returns(Task.FromResult(stubMessages as IEnumerable<Message>));

            // Act
            var messages = messageLogger
                .GetMessageLogs(userId, fromDate, toDate, functionId, messengerType)
                .GetAwaiter()
                .GetResult();

            // Assert
            var firstMessage = messages.First();
            Assert.Single(messages);
            Assert.Equal("content", firstMessage.Content);
            Assert.Equal("messengername", firstMessage.MessengerUsername);
            Assert.Equal(new DateTime(1989, 12, 13), firstMessage.SentDate);
            Assert.Equal(1, firstMessage.Id);
            Assert.Equal(2, firstMessage.Sender);
            Assert.Equal(3, firstMessage.Receiver);
        }

        [Fact]
        public void GetMessageLogs_FromDateIsLowerMinDate_ExecuteGetMessageWithMinDate()
        {
            var currentDate = new DateTime(1989, 12, 12);
            getNow.Invoke().Returns(currentDate);
            chatbotConfigs.MaxGetMessageLogDays.Returns(3);
            var fromDate = new DateTime(1989, 12, 7);

            messageLogger
                .GetMessageLogs(1, fromDate, DateTime.Now, 1, 1)
                .GetAwaiter()
                .GetResult();

            dynamicRepository.Received().FetchAsync<Message>(Arg.Is<GetMessageLogCriteria>(
                criteria => criteria.FromDate == new DateTime(1989, 12, 9)));
        }

        [Fact]
        public void LogMessage_Always_ExecuteInsertMessageCommand()
        {
            // Arrange
            var sender = 1;
            var receiver = 2;
            var messenger = new Messenger(1, "username", true, false);
            var message = "message content";
            var status = 1;
            var functionid = 2;

            // Act
            messageLogger.LogMessage(sender, receiver, messenger, message, status, functionid);

            // Assert
            dynamicRepository.ExecuteAsync(Arg.Is<InsertMessageCommand>(
                command => command.Sender == sender
                            && command.Receiver == receiver
                            && command.MessengerType == messenger.Type
                            && command.MessengerUsername == messenger.Username
                            && command.Status == status
                            && command.FunctionId == functionid));
        }

        [Fact]
        public void LogMessage_MarkupMessage_ExecuteInsertMessageCommand()
        {
            // Arrange
            var sender = 1;
            var receiver = 2;
            var messenger = new Messenger(1, "username", true, false);
            var status = 1;
            var functionid = 2;
            var message = new MessengerClient.Models.Message
            {
                Content = "MessageContent",
                MessageMarkup = new MessengerClient.Models.MessageMarkup
                {
                    MessageActions = new List<MessengerClient.Models.MessageAction>
                    {
                        new MessengerClient.Models.MessageAction{ Label="Label1", Data="Data1"},
                        new MessengerClient.Models.MessageAction{ Label="Label2", Data="Data2"}
                    }
                }
            };
            var expectedLogMessage = "MessageContent<br>"
                + "<a href='#' data-hidden='Data1'>Label1</a><br>"
                + "<a href='#' data-hidden='Data2'>Label2</a>";

            // Act
            messageLogger.LogMessage(sender, receiver, messenger, message, status, functionid);

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertMessageCommand>(
                c => c.Sender == sender
                    && c.Receiver == receiver
                    && c.Message == expectedLogMessage
                    && c.MessengerType == messenger.Type
                    && c.MessengerUsername == messenger.Username
                    && c.Status == status
                    && c.FunctionId == functionid));
        }
    }
}