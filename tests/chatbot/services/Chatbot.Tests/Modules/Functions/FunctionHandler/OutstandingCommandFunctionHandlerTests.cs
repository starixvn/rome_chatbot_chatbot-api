namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    public class OutstandingCommandFunctionHandlerTests
    {
        private readonly ILogger logger;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly CustomerService customerService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;
        private readonly OutstandingCommandFunctionHandler handler;
        private readonly IChatbotConfigs chatbotConfigs;

        public OutstandingCommandFunctionHandlerTests()
        {
            logger = Substitute.For<ILogger>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageSender = Substitute.For<IMessageSender>();
            customerService = Substitute.For<CustomerService>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();

            handler = new OutstandingCommandFunctionHandler(
                messageBuilder,
                messageSender,
                customerService,
                logger,
                chatbotConfigs,
                getDataHandlers);
        }

        [Fact]
        public void Handle_ParameterIsNumer_RunDataHandlerAndSendMessage()
        {
            // Arrange
            var param = new FunctionParam(
                new User(1, 2, 4, 3, "en-US"),
                new Function { Id = 1 },
                new Messenger(1, "sa"),
                new Dictionary<string, object> { ["parameter"] = 10 });

            getDataHandlers.Add(1, async (_) => await Task.FromResult("Message"));
            customerService.IsAccountClosed(2, 3).Returns(false);
            messageBuilder.BuildCommandResultMessage("en-US", 1, 1, "Message").Returns("Message");

            // Act
            handler.Handle(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(1, param.Messenger, "Message", 1);
        }

        [Fact]
        public void Handle_InvalidUserName_BuildAndSendInvalidCustNameMessage()
        {
            // Arrange
            var param = new FunctionParam(
                new User(1, 2, 4, 3, "en-US"),
                new Function { Id = 1 },
                new Messenger(1, "sa"),
                new Dictionary<string, object> { ["parameter"] = "abc" });

            getDataHandlers.Add(1, async (_) => await Task.FromResult("Message"));
            customerService.IsAccountClosed(2, 3).Returns(false);

            var downlineInfo = new DownlineInfo();
            customerService.GetDownlineInfo(2, "abc").Returns(Task.FromResult(downlineInfo));
            messageBuilder.BuildInvalidCustNameMessage("en-US").Returns("Invalid username");

            // Act
            handler.Handle(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(1, param.Messenger, "Invalid username", 1);
        }

        [Fact]
        public void Handle_ValidUserNameAndIsNotDirectDownline_BuildAndSendInvalidCustNameMessage()
        {
            // Arrange
            var param = new FunctionParam(
                new User(1, 2, 4, 3, "en-US"),
                new Function { Id = 1 },
                new Messenger(1, "sa"),
                new Dictionary<string, object> { ["parameter"] = "abc" });

            getDataHandlers.Add(1, async (_) => await Task.FromResult("Message"));
            customerService.IsAccountClosed(2, 3).Returns(false);

            var downlineInfo = new DownlineInfo { CustId = 5, RoleId = 2 };
            customerService.GetDownlineInfo(2, "abc").Returns(Task.FromResult(downlineInfo));
            messageBuilder.BuildInvalidCustNameMessage("en-US").Returns("Invalid username");

            // Act
            handler.Handle(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(1, param.Messenger, "Invalid username", 1);
        }

        [Fact]
        public void Handle_ValidUserNameAndIsNotDirectDownline_RunDataHandlerAndSendMessage()
        {
            // Arrange
            var param = new FunctionParam(
                new User(1, 2, 4, 3, "en-US"),
                new Function { Id = 1 },
                new Messenger(1, "sa"),
                new Dictionary<string, object> { ["parameter"] = "abc" });

            getDataHandlers.Add(1, async (_) => await Task.FromResult("Message"));
            customerService.IsAccountClosed(2, 3).Returns(false);

            var downlineInfo = new DownlineInfo { CustId = 5, RoleId = 3 };
            customerService.GetDownlineInfo(2, "abc").Returns(Task.FromResult(downlineInfo));
            messageBuilder.BuildCommandResultMessage("en-US", 1, 1, "Message").Returns("Message");

            // Act
            handler.Handle(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(1, param.Messenger, "Message", 1);
        }
    }
}