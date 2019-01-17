namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Functions/Handlers")]
    public class FunctionHandlerFactoryTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> subDictionary;
        private readonly IChatbotConfigs subChatbotConfigs;
        private readonly CustomerService subCustomerService;
        private readonly CopyCustomerService subCopyCustomerService;
        private readonly SettingService subSettingService;
        private readonly UserLogActionService userLogActionService;
        private readonly ILogger subLogger;
        private readonly IMessageLogger messageLogger;

        public FunctionHandlerFactoryTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subDictionary = Substitute.For<IDictionary<int, Func<FunctionParam, Task<object>>>>();
            subChatbotConfigs = Substitute.For<IChatbotConfigs>();
            subCustomerService = Substitute.For<CustomerService>();
            subSettingService = Substitute.For<SettingService>();
            subCopyCustomerService = Substitute.For<CopyCustomerService>();
            userLogActionService = Substitute.For<UserLogActionService>();
            messageLogger = Substitute.For<IMessageLogger>();
            subLogger = Substitute.For<ILogger>();
            subChatbotConfigs.MessageAndAnnoucementFunctionIds.Returns(new List<int> { 5, 6, 7, 8 });
            subChatbotConfigs.YesterdayWinLossFunctionId.Returns(4);
            subChatbotConfigs.OutstandingFunctionId.Returns(1);
            subChatbotConfigs.CopyCustomerFunctionId.Returns(9);
            subChatbotConfigs.BalanceFunctionId.Returns(3);
        }

        private FunctionHandlerFactory CreateFactory()
        {
            return new FunctionHandlerFactory(
                subMessageBuilder,
                subMessageSender,
                subDictionary,
                subChatbotConfigs,
                subCustomerService,
                subSettingService,
                subCopyCustomerService,
                userLogActionService,
                messageLogger,
                subLogger);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void CreateFunctionHandler_FunctionIdIsMessageAndAnnouncement_ReturnMessageAndAnnouncementHandler(int functionId)
        {
            // Arrange

            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(functionId);

            // Assert
            Assert.IsType<MessageAnnouncementFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_FunctionIdIsYesterdayWinLoss_ReturnYesterdayWinLossFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(4);

            // Assert
            Assert.IsType<YesterdayWinLossFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_FunctionIdIsOutstanding_ReturnOutstandingFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(1);

            // Assert
            Assert.IsType<OutstandingFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_FunctionIdIsCopyCustomer_ReturnCopyCustomerFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(9);

            // Assert
            Assert.IsType<CopyCustomerFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_FunctionIdIsBalanceBalanceFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(3);

            // Assert
            Assert.IsType<BalanceFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_OtherFunctionId_ReturnDefaultFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(2);

            // Assert
            Assert.IsType<DefaultFunctionHandler>(functionHandler);
        }
    }
}