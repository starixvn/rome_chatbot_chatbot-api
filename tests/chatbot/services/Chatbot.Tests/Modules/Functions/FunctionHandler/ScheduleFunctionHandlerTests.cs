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
    using NSubstitute;
    using Xunit;

    public class ScheduleFunctionHandlerTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly CustomerService subCustomerService;
        private readonly SettingService subSettingService;
        private readonly ILogger subLogger;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        public ScheduleFunctionHandlerTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subCustomerService = Substitute.For<CustomerService>();
            subSettingService = Substitute.For<SettingService>();
            subLogger = Substitute.For<ILogger>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();
        }

        private YesterdayWinLossFunctionHandler CreateScheduleFunctionHandler()
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
        public async Task Handle_IsCustomerClosed_GetResult()
        {
            // Arrange
            var param = new FunctionParam(new User(1, custId: 2, custLevel: 4, subAccountId: 3, language: "en-US"),
                new Function { Id = 2 }, new Models.Messenger(), null);
            getDataHandlers.Add(2, null);
            subCustomerService.IsAccountClosed(2, 3).Returns(true);
            var handler = CreateScheduleFunctionHandler();

            // Act
            var result = await handler.Handle(param);

            // Assert
            Assert.Equal("Account is closed", result.Message);
        }
    }
}