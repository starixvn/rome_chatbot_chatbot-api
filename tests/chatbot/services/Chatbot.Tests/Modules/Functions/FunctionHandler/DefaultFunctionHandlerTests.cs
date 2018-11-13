namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Functions/Handlers")]
    public class DefaultFunctionHandlerTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly CustomerService subCustomerService;
        private readonly ILogger subLogger;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        public DefaultFunctionHandlerTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subCustomerService = Substitute.For<CustomerService>();
            subLogger = Substitute.For<ILogger>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();
        }

        private DefaultFunctionHandler CreateDefaultFunctionHandler()
        {
            return new DefaultFunctionHandler(
                subMessageBuilder,
                subMessageSender,
                subCustomerService,
                subLogger,
                getDataHandlers);
        }

        [Fact]
        public void Handle_GetDataHandlerDoesNotContainKey_ThrowException()
        {
            // Arrange
            var param = new FunctionParam(new Models.User(1, 2, 3, 4, "en-US", null), new Models.Function { Id = 1 }, new Models.Messenger(), null);
            var handler = CreateDefaultFunctionHandler();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => handler.Handle(param).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Handle_IsCustomerClosed_BuildAndSendAccountClosedMessage()
        {
            // Arrange
            var param = new FunctionParam(new Models.User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US", selfdestructionPeriod: null),
                new Models.Function { Id = 2 }, new Models.Messenger(), null);
            getDataHandlers.Add(2, null);
            subCustomerService.IsAccountClosed(2, 3).Returns(true);
            subMessageBuilder.BuildAccountClosedMessage("en-US").Returns("Account is closed");
            var handler = CreateDefaultFunctionHandler();

            // Act
            await handler.Handle(param);

            // Assert
            await subMessageSender.Received().SendMessage(1, param.Messenger, "Account is closed", 2);
        }

        [Fact]
        public async Task Handle_IsNormal_RunDataHandlerAndSendMessage()
        {
            // Arrange
            var param = new FunctionParam(new Models.User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US", selfdestructionPeriod: null),
                new Models.Function { Id = 2 }, new Models.Messenger(1, "sa"), null);
            getDataHandlers.Add(2, async (_) => await Task.FromResult("Message"));
            subCustomerService.IsAccountClosed(2, 3).Returns(false);
            subMessageBuilder.BuildCommandResultMessage("en-US", 2, 1, "Message").Returns("Message");
            var handler = CreateDefaultFunctionHandler();

            // Act
            await handler.Handle(param);

            // Assert
            await subMessageSender.Received().SendMessage(1, param.Messenger, "Message", 2);
        }
    }
}