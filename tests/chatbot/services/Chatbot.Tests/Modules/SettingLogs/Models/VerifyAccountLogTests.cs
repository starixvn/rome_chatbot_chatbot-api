namespace Chatbot.Tests.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using Chatbot.Shared.Constants;
    using Chatbot.Models;
    using Chatbot.Modules.SettingLogs.Constants;
    using Chatbot.Modules.SettingLogs.Models;
    using Xunit;

    [Trait("Modules", "SettingLogs")]
    public class VerifyAccountLogTests
    {
        [Fact]
        public void SPName_Always_ReturnsCorrectValue()
        {
            // Act
            var linkAccountLog = new VerifyAccountLog(1, 2, "127.0.0.1", null);

            // Assert
            Assert.Equal("Chatbot_UpdateMessengerAccountStatus", linkAccountLog.SPName);
        }

        [Theory]
        [InlineData(MessengerType.Letstalk, LogAction.VerifyLetstalkId)]
        public void ActionId_Always_ReturnCorrectValue(int messengerType, int expectedActionId)
        {
            // Act
            var linkAccountLog = new VerifyAccountLog(1, 2, "127.0.0.1", new Messenger(messengerType, string.Empty));

            // Assert
            Assert.Equal(expectedActionId, linkAccountLog.ActionId);
        }

        [Fact]
        public void NewValues_Always_ReturnsCorrectValue()
        {
            // Arrange
            var messenger = new Messenger(MessengerType.Letstalk, "UserName");
            var expectedResult = new Dictionary<string, object>
            {
                [nameof(messenger.Username)] = messenger.Username
            };

            // Act
            var linkAccountLog = new VerifyAccountLog(1, 2, "127.0.0.1", messenger);

            // Assert
            Assert.Equal(expectedResult, linkAccountLog.NewValues);
        }
    }
}