using Chatbot.Modules.Funtions;
using Chatbot.Modules.MessageSenders;
using Chatbot.Modules.MessageServices;
using Chatbot.Modules.Settings;
using Chatbot.WebApi.Controllers;
using Chatbot.WebApi.Params;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Chatbot.WebApi.Tests.Controllers
{
    public class MessageControllerTests
    {
        private IMessageSender subMessageSender;
        private SettingService subSettingService;
        private FunctionService subFunctionService;
        private IMessageLogger subMessageLogger;

        public MessageControllerTests()
        {
            this.subMessageSender = Substitute.For<IMessageSender>();
            this.subSettingService = Substitute.For<SettingService>();
            this.subFunctionService = Substitute.For<FunctionService>();
            this.subMessageLogger = Substitute.For<IMessageLogger>();
        }

        private MessageController CreateMessageController()
        {
            return new MessageController(
                this.subMessageSender,
                this.subSettingService,
                this.subFunctionService,
                this.subMessageLogger);
        }

        [Fact]
        public async Task SendToSpecificMessenger_StateUnderTest_ExpectedBehavior()
        {
        }

        [Fact]
        public async Task Send_StateUnderTest_ExpectedBehavior()
        {
        }

        [Fact]
        public async Task GetFunctionMessages_StateUnderTest_ExpectedBehavior()
        {
        }

        [Fact]
        public async Task GetMessageLogs_StateUnderTest_ExpectedBehavior()
        {
        }
    }
}