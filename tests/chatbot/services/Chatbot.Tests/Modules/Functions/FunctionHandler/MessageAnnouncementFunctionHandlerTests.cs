namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using NSubstitute;
    using Xunit;

    public class MessageAnnouncementFunctionHandlerTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly CustomerService subCustomerService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        public MessageAnnouncementFunctionHandlerTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subCustomerService = Substitute.For<CustomerService>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();
        }

        private MessageAnnouncementFunctionHandler CreateMessageAnnouncementFunctionHandler()
        {
            return new MessageAnnouncementFunctionHandler(
                subMessageBuilder,
                subMessageSender,
                subCustomerService,
                getDataHandlers);
        }

        [Fact]
        public void Handle_GetDataHandlerDoesNotContainKey_ThrowException()
        {
            // Arrange
            var param = new FunctionParam(new Models.User(1, 2, 3, 4, "en-US"), new Models.Function { Id = 1 }, new Models.Messenger(), null);
            var handler = CreateMessageAnnouncementFunctionHandler();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => handler.Handle(param).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Handle_IsCustomerClosed_GetResult()
        {
            // Arrange
            var param = new FunctionParam(new User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US"),
                new Function { Id = 2 }, new Models.Messenger(), null);
            getDataHandlers.Add(2, null);
            subCustomerService.IsAccountClosed(2, 3).Returns(true);
            var handler = CreateMessageAnnouncementFunctionHandler();

            // Act
            var result = await handler.Handle(param);

            // Assert
            Assert.Equal("Customer is closed", result.Message);
        }

        [Fact]
        public async Task Handle_MessageListHasNoItem_ReturnResultNoData()
        {
            // Arrange
            var param = new FunctionParam(new User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US"),
                new Function { Id = 2 }, new Models.Messenger(1, "sa"), null);
            getDataHandlers.Add(2, async (_) => await Task.FromResult(new List<Alpha.Services.Client.Models.Message>()));
            subCustomerService.IsAccountClosed(2, 3).Returns(false);
            subMessageBuilder.BuildCommandResultMessage("en-US", 2, 1, "Message").Returns("Message");
            var handler = CreateMessageAnnouncementFunctionHandler();

            // Act
            var result = await handler.Handle(param);

            // Assert
            Assert.Equal("No data", result.Message);
        }

        [Fact]
        public async Task Handle_MessageListHasData_BuildAndSendMessage()
        {
            // Arrange
            var param = new FunctionParam(new User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US"),
                new Function { Id = 2 }, new Models.Messenger(1, "sa"), null);
            getDataHandlers.Add(2, async (_) => await Task.FromResult(
                new List<Alpha.Services.Client.Models.Message> {
                    new Alpha.Services.Client.Models.Message { Content = "Test Message" },
                    new Alpha.Services.Client.Models.Message { Content = "" }
                }));
            subCustomerService.IsAccountClosed(2, 3).Returns(false);
            subMessageBuilder.BuildCommandResultMessage("en-US", 2, 1, Arg.Any<object>()).Returns("Test Message");
            subMessageSender.SendMessage(1, param.Messenger, "Test Message", 2).Returns(Result.CreateSuccessfulResult());
            var handler = CreateMessageAnnouncementFunctionHandler();

            // Act
            var result = await handler.Handle(param);

            // Assert
            await subMessageSender.Received().SendMessage(1, param.Messenger, "Test Message", 2);
            Assert.True(result.IsOk);
        }
    }
}