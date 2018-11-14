namespace Chatbot.Tests.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using Chatbot.Modules.SettingLogs.Constants;
    using Chatbot.Modules.SettingLogs.Models;
    using Xunit;

    [Trait("Modules", "SettingLogs")]
    public class UpdateLanguageLogTests
    {
        [Fact]
        public void SPName_Always_ReturnsCorrectValue()
        {
            // Act
            var linkAccountLog = new UpdateLanguageLog(1, 2, "127.0.0.1", "en-US", "zh-CN");

            // Assert
            Assert.Equal("Chatbot_UpdateUserLanguage", linkAccountLog.SPName);
        }

        [Fact]
        public void ActionId_Always_ReturnsCorrectValue()
        {
            // Act
            var linkAccountLog = new UpdateLanguageLog(1, 2, "127.0.0.1", "en-US", "zh-CN");

            // Assert
            Assert.Equal(LogAction.UpdateChatbotLanguage, linkAccountLog.ActionId);
        }

        [Fact]
        public void NewValues_Always_ReturnsCorrectValue()
        {
            var newLanguage = "en-US";
            var expectedResult = new Dictionary<string, object>()
            {
                ["Language"] = newLanguage
            };

            // Act
            var linkAccountLog = new UpdateLanguageLog(1, 2, "127.0.0.1", newLanguage, "zh-CN");

            // Assert
            Assert.Equal(expectedResult, linkAccountLog.NewValues);
        }

        [Fact]
        public void OldValues_Always_ReturnsCorrectValue()
        {
            var oldLanguage = "zh-CN";
            var expectedResult = new Dictionary<string, object>()
            {
                ["Language"] = oldLanguage
            };

            // Act
            var linkAccountLog = new UpdateLanguageLog(1, 2, "127.0.0.1", "en-US", oldLanguage);

            // Assert
            Assert.Equal(expectedResult, linkAccountLog.OldValues);
        }
    }
}