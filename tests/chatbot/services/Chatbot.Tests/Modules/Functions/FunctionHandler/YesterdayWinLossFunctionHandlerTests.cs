namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;
    using AccountStatus = Alpha.Services.Client.Models.AccountStatus;

    [Trait("Modules", "Functions/Handlers")]
    public class YesterdayWinLossFunctionHandlerTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly CustomerService subCustomerService;
        private readonly SettingService subSettingService;
        private readonly ILogger subLogger;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;
        private readonly CompareLogic comparer;

        public YesterdayWinLossFunctionHandlerTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subCustomerService = Substitute.For<CustomerService>();
            subSettingService = Substitute.For<SettingService>();
            subLogger = Substitute.For<ILogger>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>()
            {
                {4, GetMessage}
            };
            comparer = new CompareLogic();
        }

        private YesterdayWinLossFunctionHandler CreateYesterdayFunctionHandler()
        {
            return new YesterdayWinLossFunctionHandler(
                subMessageBuilder,
                subMessageSender,
                subCustomerService,
                subSettingService,
                subLogger,
                getDataHandlers);
        }

        [Fact]
        public async Task GetFunctionMessages_CustomerIsNotClosed_BuildFunctionMessage()
        {
            // Arrange
            subSettingService.GetMessengers(2, true).Returns(new List<Messenger> { new Messenger(2, "userName") });
            subMessageBuilder.BuildCommandResultMessage("en-US", 4, 2, Arg.Any<object>()).Returns("Message");
            subCustomerService.GetAccountStatus(1, 1).Returns(new AccountStatus { IsActive = true });
            var handler = CreateYesterdayFunctionHandler();

            // Act
            var functionMessages = await handler.GetFunctionMessages(
                new FunctionParam(null, new Function { Id = 4 }, null, null, new List<User> { new User(2, 1, 1, 1, "en-US", null) }));

            // Assert
            var expectedFunctionMessages = new List<FunctionMessage> { new FunctionMessage(2, "Message", null) };
            Assert.True(comparer.Compare(expectedFunctionMessages, functionMessages).AreEqual);
        }

        [Fact]
        public async Task GetFunctionMessages_CustomerIsClosed_NoyBuildFunctionMessage()
        {
            // Arrange
            subSettingService.GetMessengers(2, true).Returns(new List<Messenger> { new Messenger(2, "userName") });
            subMessageBuilder.BuildCommandResultMessage("en-US", 4, 2, Arg.Any<object>()).Returns("Message");
            subCustomerService.GetAccountStatus(1, 1).Returns(new AccountStatus());
            var handler = CreateYesterdayFunctionHandler();

            // Act
            var functionMessages = await handler.GetFunctionMessages(
                new FunctionParam(null, new Function { Id = 4 }, null, null, new List<User> { new User(2, 1, 1, 1, "en-US", null) }));

            // Assert
            var expectedFunctionMessages = new List<FunctionMessage>();
            Assert.True(comparer.Compare(expectedFunctionMessages, functionMessages).AreEqual);
        }

        private async Task<object> GetMessage(FunctionParam param)
        {
            return await Task.Run(() => new List<Alpha.Services.Client.Models.Message>
            {
                new Alpha.Services.Client.Models.Message{ Content = "Message", CustId = 1}
            }).ConfigureAwait(false);
        }
    }
}