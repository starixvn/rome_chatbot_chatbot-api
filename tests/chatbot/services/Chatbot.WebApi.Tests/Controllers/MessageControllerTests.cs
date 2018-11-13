namespace Chatbot.WebApi.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Chatbot.WebApi.Controllers;
    using Chatbot.WebApi.Params;
    using NSubstitute;
    using Xunit;

    [Trait("Controllers", "Message")]
    public class MessageControllerTests
    {
        private readonly IMessageSender subMessageSender;
        private readonly SettingService subSettingService;
        private readonly FunctionService subFunctionService;
        private readonly IMessageLogger subMessageLogger;
        private readonly MessageController messageController;

        public MessageControllerTests()
        {
            subMessageSender = Substitute.For<IMessageSender>();
            subSettingService = Substitute.For<SettingService>();
            subFunctionService = Substitute.For<FunctionService>();
            subMessageLogger = Substitute.For<IMessageLogger>();
            messageController = new MessageController(
                subMessageSender,
                subSettingService,
                subFunctionService,
                subMessageLogger);
        }

        [Fact]
        public void SendToSpecificMessenger_Always_CallsSendMessage()
        {
            // Arrange
            var param = new MessageParam
            {
                UserId = 1,
                MessengerType = 1,
                MessengerUsername = "username",
                Message = "message",
                SelfdestructionPeriod = null
            };

            // Act
            messageController.SendToSpecificMessenger(param).ConfigureAwait(false);

            // Assert
            subMessageSender.Received().SendMessage(
                param.UserId,
                Arg.Is<Models.Messenger>(
                    m => m.Type == param.MessengerType
                        && m.Username == param.MessengerUsername),
                param.Message,
                param.SelfdestructionPeriod);
        }

        [Fact]
        public void Send_Always_CallsGetMessengers()
        {
            // Arrange
            var param = new MessageParam
            {
                UserId = 1,
                FunctionId = 1,
                Message = "message"
            };

            // Act
            messageController.Send(param).ConfigureAwait(false);

            // Assert
            subSettingService.Received().GetMessengers(param.UserId, true);
        }

        [Fact]
        public void Send_Always_CallsSendMessage()
        {
            // Arrange
            var param = new MessageParam
            {
                UserId = 1,
                FunctionId = 1,
                Message = "message"
            };
            IEnumerable<Models.Messenger> messengers = new List<Models.Messenger>
            {
                new Models.Messenger(1,"username")
            };
            subSettingService.GetMessengers(param.UserId, true).Returns(Task.FromResult(messengers));

            // Act
            messageController.Send(param).ConfigureAwait(false);

            // Assert
            subMessageSender.Received().SendMessage(
                param.UserId,
                param.Message,
                messengers,
                param.SelfdestructionPeriod,
                param.FunctionId);
        }

        [Fact]
        public void GetFunctionMessages_Always_GetFunctionMessages()
        {
            // Arrange
            var functionId = 1;

            // Act
            messageController.GetFunctionMessages(functionId).ConfigureAwait(false);

            // Assert
            subFunctionService.GetFunctionMessages(functionId);
        }

        [Fact]
        public void GetMessageLogs_Always_GetMessageLogs()
        {
            // Arrange
            var fromDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var toDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var param = new MessageLogParam
            {
                UserId = 1,
                FromDate = fromDate,
                ToDate = toDate,
                FunctionId = 1,
                MessengerType = 1
            };

            // Act
            messageController.GetMessageLogs(param).ConfigureAwait(false);

            // Assert
            subMessageLogger.GetMessageLogs(
                param.UserId,
                DateTimeOffset.FromUnixTimeMilliseconds(param.FromDate).DateTime.ToLocalTime(),
                DateTimeOffset.FromUnixTimeMilliseconds(param.ToDate).DateTime.ToLocalTime(),
                param.FunctionId,
                param.MessengerType);
        }
    }
}