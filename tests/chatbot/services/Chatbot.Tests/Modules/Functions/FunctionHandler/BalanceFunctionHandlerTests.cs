using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alpha.Services.Client.Services;
using Chatbot.Modules.Functions.FunctionHandler;
using Chatbot.Modules.Funtions.Params;
using Chatbot.Modules.MessageBuilders;
using Chatbot.Modules.MessageSenders;
using Chatbot.Shared.Configurations;
using Fanex.Logging;
using NSubstitute;
using Xunit;

namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    public class BalanceFunctionHandlerTests
    {
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly CustomerService customerService;
        private readonly ILogger logger;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;
        private readonly BalanceFunctionHandler balanceFunctionHandler;

        public BalanceFunctionHandlerTests()
        {
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            chatbotConfigs.BalanceMaxTopQuery.Returns(10);

            messageBuilder = Substitute.For<IMessageBuilder>();
            messageSender = Substitute.For<IMessageSender>();
            customerService = Substitute.For<CustomerService>();
            logger = Substitute.For<ILogger>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();

            balanceFunctionHandler = new BalanceFunctionHandler(
                chatbotConfigs,
                messageBuilder,
                messageSender,
                customerService,
                logger,
                getDataHandlers);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void RunHandler_TopQueryIsInvalid_BuildAndSendInvalidNumerOfDownlineRangeMessage(int topQuery)
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others.Add("parameters", new string[] { $"{topQuery}" });
            var message = "DownlineRangeMessage";
            messageBuilder.BuildInvalidNumerOfDownlineRange(param.User.Language, chatbotConfigs.BalanceMaxTopQuery)
                .Returns(message);

            // Act
            balanceFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        private static FunctionParam CreateDefaultFunctionParam()
            => new FunctionParam(
                new Models.User(1, 2, 3, 0, "en-US", 15),
                new Models.Function { Id = 1 },
                new Models.Messenger(1, "username"),
                new Dictionary<string, object>());
    }
}