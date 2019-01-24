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
                "Your Letstalk ID has been verified, Atom is fully functioning._newline_You can turn off any feature via the Chatbot configuration page on your agency site._newline_Hint: you could input any word to get my suggestion right away.",
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

        [Fact]
        public void BuildInvalidNumerOfDownline_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildInvalidNumerOfDownline(language);

            // Assert
            Assert.Equal("Number of downlines must be numeric.", message);
        }

        [Fact]
        public void BuildInvalidNumerOfDownlineRange_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildInvalidNumerOfDownlineRange(language, 10);

            // Assert
            Assert.Equal("Number of downlines must be from 1 to 10.", message);
        }

        [Fact]
        public void BuildInvalidDownlineMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";
            var username = "username";

            // Act
            var message = messageBuilder.BuildInvalidDownlineMessage(username, language);

            // Assert
            Assert.Equal("username is not your valid downline.", message);
        }

        [Fact]
        public void BuildInvalidNumericMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildInvalidNumericMessage("1000", language);

            // Assert
            Assert.Equal("1000 is not a numeric value.", message);
        }

        [Fact]
        public void BuildRootActionTitle_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildRootActionTitle(language);

            // Assert
            Assert.Equal("Sorry, we couldn't recognize your request. Which action would you like to take?", message);
        }

        [Fact]
        public void BuildCancelRequestMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildCancelRequestMessage(language);

            // Assert
            Assert.Equal("Cancel current request", message);
        }

        [Fact]
        public void BuildResponseCancelRequestMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildResponseCancelRequestMessage(1, language);

            // Assert
            Assert.Equal("The current request has been cancelled._newline_Which action would you like to take?", message);
        }

        [Fact]
        public void BuildConfirmKeywordTitle_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildConfirmKeywordTitle(language);

            // Assert
            Assert.Equal("Do you want to request any of the following function(s)?", message);
        }

        [Fact]
        public void BuildNoneOfTheAboveMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildNoneOfTheAboveMessage(language);

            // Assert
            Assert.Equal("None of the above", message);
        }

        [Fact]
        public void BuildCopyCustomerInformMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildCopyCustomerInformMessage(language);

            // Assert
            Assert.Equal("Your request is being processed now, please wait...", message);
        }

        [Fact]
        public void BuildSuggestionActionsTitle_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildSuggestionActionsTitle(language);

            // Assert
            Assert.Equal("Here are some actions I could do for you:", message);
        }

        [Fact]
        public void BuildCompleteUMNotification_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildCompleteUMNotification(1, language);

            // Assert
            Assert.Equal("Our UM time is over. All of our service has been back to work._newline_Hint: you could input any word to get my assistance right away.", message);
        }

        [Fact]
        public void BuildConfirmUpdateAction_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildConfirmUpdateAction("actionname", 1, language);

            // Assert
            Assert.Equal("Yes_newline_actionname", message);
        }

        [Fact]
        public void BuildCopyCustomerReachedLimitMessage_Always_ReturnMessage()
        {
            // Arrange
            var language = "en-US";

            // Act
            var message = messageBuilder.BuildCopyCustomerReachedLimitMessage(50, language);

            // Assert
            Assert.Equal("Sorry, you have reached the limit for creating new account via Letstalk today (50 accounts per day).", message);
        }
    }
}