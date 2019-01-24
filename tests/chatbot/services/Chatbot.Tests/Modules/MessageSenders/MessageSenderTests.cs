namespace Chatbot.Tests.Modules
{
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;
    using Messenger.Client;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "MessageSenders")]
    public class MessageSenderTests
    {
        private readonly MessengerFactory messengerFactory;
        private readonly IMessageLogger messageLogger;
        private readonly ILogger logger;
        private readonly MessageSender messageSender;
        private readonly IMessengerClient messengerClient;
        private readonly IChatbotConfigs chatbotConfigs;

        public MessageSenderTests()
        {
            messengerFactory = Substitute.For<MessengerFactory>();
            messageLogger = Substitute.For<IMessageLogger>();
            logger = Substitute.For<ILogger>();
            messengerClient = Substitute.For<IMessengerClient>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();

            messageSender = Substitute.ForPartsOf<MessageSender>(
                messengerFactory,
                logger,
                messageLogger,
                chatbotConfigs);
        }

        [Fact]
        public void SendMessageWithoutUserId_Always_CallSendMessageWithUserIdIsZero()
        {
            // Arrange
            var messenger = new Models.Messenger(1, "messengername");
            var message = "message content";

            // Act
            messageSender.SendMessage(messenger, message, null).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(0, messenger, message, null).ConfigureAwait(false);
        }

        [Fact]
        public void SendMessage_Always_CreateMessengerClient()
        {
            var userId = 2;
            var messenger = new Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result());
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message, null).GetAwaiter().GetResult();

            messengerFactory.Received().CreateMessengerClient(messenger.Type);
        }

        [Fact]
        public void SendMessage_PushMessageFailed_LogError()
        {
            var userId = 2;
            var messenger = new Chatbot.Models.Messenger(1, "messengername");
            var message = "message content";
            var failedResult = new Messenger.Client.Models.Result { Status = 123, Message = "Test Message" };
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(failedResult);
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message, null).GetAwaiter().GetResult();

            logger.Received().Error("Deliver message failed: message content\r\nCode: 123, Message: Test Message");
        }

        [Fact]
        public void SendMessage_SettingLogToDatabase_ExecuteLogMessage()
        {
            var userId = 2;
            var messenger = new Chatbot.Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result { Status = 1 });
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            messageSender.SendMessage(userId, messenger, message, null, 3, true).GetAwaiter().GetResult();

            messageLogger.Received().LogMessage(CommonId.BotUserId, userId, messenger, message, 1, 3);
        }

        [Fact]
        public void SendMessage_Always_ReturnSucccesResult()
        {
            var userId = 2;
            var messenger = new Models.Messenger(1, "messengername");
            var message = "message content";
            messengerClient.PushAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(new Messenger.Client.Models.Result { Status = 1 });
            messengerFactory.CreateMessengerClient(Arg.Any<int>()).Returns(messengerClient);

            var result = messageSender.SendMessage(userId, messenger, message, null, 3, true).GetAwaiter().GetResult();

            Assert.True(result.IsOk);
        }

        [Fact]
        public void IsMessengerAccountNotExisted_Always_CreateMessengerClient()
        {
            // Arrange
            var messenger = new Models.Messenger(1, "username");

            // Act
            messageSender.IsMessengerAccountNotExisted(messenger).ConfigureAwait(false);

            // Assert
            messengerFactory.Received().CreateMessengerClient(messenger.Type);
        }
    }
}