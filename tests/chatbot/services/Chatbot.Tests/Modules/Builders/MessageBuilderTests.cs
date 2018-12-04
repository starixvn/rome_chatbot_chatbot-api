namespace Chatbot.Tests.Modules
{
    using System.Collections.Generic;
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageBuilders.Templates;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Builders")]
    public class MessageBuilderTests
    {
        private readonly IDictionary<int, IMessengerFormatter> messengerFormatters;
        private readonly IDictionary<string, CultureInfo> cultureInfoList;
        private readonly IDictionary<int, IMessageTemplate> messageTemplates;
        private readonly IMessengerFormatter messengerFormatter;
        private readonly IMessageTemplate messageTemplate;
        private readonly MessageBuilder messageBuilder;

        public MessageBuilderTests()
        {
            messageTemplate = Substitute.For<IMessageTemplate>();
            messengerFormatter = Substitute.For<IMessengerFormatter>();
            messengerFormatters = new Dictionary<int, IMessengerFormatter>
            {
                { 1, messengerFormatter }
            };
            messageTemplates = new Dictionary<int, IMessageTemplate>
            {
                { 1, messageTemplate }
            };
            cultureInfoList = new Dictionary<string, CultureInfo>();
            messengerFormatter.NewLine().Returns("_newline_");

            messageBuilder = new MessageBuilder(cultureInfoList, messageTemplates, messengerFormatters);
        }

        [Fact]
        public void BuildVerificationCodeMessage_Always_ReturnMessage()
        {
            var language = "en-US";
            var messengerType = 1;
            var verificationCode = "422134";
            var expiredVerificationCodeInMinutes = 10;

            var verificationCodeMessage = messageBuilder.BuildVerificationCodeMessage(
                language,
                messengerType,
                verificationCode,
                expiredVerificationCodeInMinutes);

            Assert.Equal(
                "You have requested to set up Atom. "
                + "Your verification code is 422134._newline_The code will expire in 10 minutes.",
                verificationCodeMessage);
        }

        [Fact]
        public void BuildNotRecognizedCommandMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildNotRecognizedCommandMessage(language);

            Assert.Equal(
                "Invalid command.",
                message);
        }

        [Fact]
        public void BuildWelcomeMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildWelcomeMessage(language, 1);

            Assert.Equal(
                "Your Letstalk ID has been verified, Atom is fully functioning._newline_You can turn off any feature via the Chatbot configuration page on your agency site.",
                message);
        }

        [Fact]
        public void BuildAccountClosedMessage_Always_ReturnMessage()
        {
            var language = "en-US";

            var message = messageBuilder.BuildAccountClosedMessage(language);

            Assert.Equal(
                "Your account is closed.",
                message);
        }

        [Fact]
        public void BuildUnderMaintenanceMessage_Always_ReturnMessage()
        {
            var message = messageBuilder.BuildUnderMaintenanceMessage(1, "2:00", "3:00");

            Assert.Equal(
                "Sorry, the system is under maintenance from 2:00 to 3:00 GMT+8. Please come back later._newline__newline_很抱歉，服务器正在维护中，维护时间从2:00至3:00 (GMT+8)。请稍后访问。_newline__newline_很抱歉，伺服器正在進行維護，維護時間從2:00至3:00 (GMT+8)。請稍候再試。_newline__newline_",
                message);
        }

        [Fact]
        public void BuildCommandResultMessage_Always_ReturnMessage()
        {
            var functionId = 1;
            var data = new Dictionary<string, object>
            {
                { "key", "value" }
            };
            var messengerType = 1;
            var language = "en-US";

            messageTemplate
                .ToString(functionId, data, Arg.Any<CultureInfo>(), messengerFormatter)
                .Returns("Builded Message");

            var message = messageBuilder.BuildCommandResultMessage(language, functionId, messengerType, data);

            Assert.Equal("Builded Message", message);
        }

        [Fact]
        public void BuildCommandResultMessage_DataIsNull_ReturnMessage()
        {
            // Arrange
            var functionId = 1;
            var messengerType = 1;
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildCommandResultMessage(language, functionId, messengerType, null);

            // Assert
            Assert.Equal("No information is available", message);
        }

        [Fact]
        public void BuildErrorMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildErrorMessage(language);

            // Assert
            Assert.Equal("Oops...you are here because of a system error, please notify the administrators", message);
        }

        [Fact]
        public void BuildServiceIsUnavailableMessage_Always_ReturnMessage()
        {
            // Arrange
            var messageType = 1;

            // Act
            var message = messageBuilder.BuildServiceIsUnavailableMessage(messageType);

            // Assert
            Assert.Equal("Sorry, you haven't registered for this chatbot service yet!_newline__newline_对不起，您尚未注册Chatbot服务!_newline__newline_對不起，您尚未註冊Chatbot服務!_newline__newline_Xin lỗi, bạn chưa đăng ký dịch vụ chatbot này!_newline__newline_", message);
        }

        [Fact]
        public void BuildInvalidCustNameMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildInvalidCustNameMessage(language);

            // Assert
            Assert.Equal("Invalid username.", message);
        }

        [Fact]
        public void BuildUnlinkMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildUnlinkMessage(language);

            // Assert
            Assert.Equal("You have opted out service successfully. Thanks a lot for using our service.", message);
        }
    }
}