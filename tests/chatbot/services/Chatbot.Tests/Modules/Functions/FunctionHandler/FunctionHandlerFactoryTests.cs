namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Shared.Configurations;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    public class FunctionHandlerFactoryTests
    {
        private readonly IMessageBuilder subMessageBuilder;
        private readonly IMessageSender subMessageSender;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> subDictionary;
        private readonly IChatbotConfigs subChatbotConfigs;
        private readonly CustomerService subCustomerService;
        private readonly SettingService subSettingService;
        private readonly ILogger subLogger;

        public FunctionHandlerFactoryTests()
        {
            subMessageBuilder = Substitute.For<IMessageBuilder>();
            subMessageSender = Substitute.For<IMessageSender>();
            subDictionary = Substitute.For<IDictionary<int, Func<FunctionParam, Task<object>>>>();
            subChatbotConfigs = Substitute.For<IChatbotConfigs>();
            subCustomerService = Substitute.For<CustomerService>();
            subSettingService = Substitute.For<SettingService>();
            subLogger = Substitute.For<ILogger>();
            subChatbotConfigs.MessageAndAnnoucementFunctionIds.Returns(new List<int> { 2, 3, 4 });
            subChatbotConfigs.YesterdayWinLossFunctionId.Returns(6);
            subChatbotConfigs.OutstandingFunctionId.Returns(1);
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
                subLogger);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
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
            var functionHandler = factory.CreateFunctionHandler(6);

            // Assert
            Assert.IsType<YesterdayWinLossFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_FunctionIdIsOutstanding_ReturnYesterdayWinLossFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(1);

            // Assert
            Assert.IsType<OutstandingCommandFunctionHandler>(functionHandler);
        }

        [Fact]
        public void CreateFunctionHandler_OtherFunctionId_ReturnDefaultFunctionHandler()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var functionHandler = factory.CreateFunctionHandler(7);

            // Assert
            Assert.IsType<DefaultFunctionHandler>(functionHandler);
        }
    }
}