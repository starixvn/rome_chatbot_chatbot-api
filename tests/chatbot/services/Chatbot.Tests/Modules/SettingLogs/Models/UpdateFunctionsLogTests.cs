using System.Linq;
using Chatbot.Models;
using Chatbot.Modules.SettingLogs.Constants;
using Chatbot.Modules.SettingLogs.Models;
using Xunit;

namespace Chatbot.Tests.Modules.SettingLogs.Models
{
    public class UpdateFunctionsLogTests
    {
        [Fact]
        public void SPName_Always_ReturnsCorrectValue()
        {
            // Act
            var linkAccountLog = new UpdateFunctionsLog(1, 2, "127.0.0.1", Enumerable.Empty<Function>(), Enumerable.Empty<Function>());

            // Assert
            Assert.Equal("Chatbot_UpdateUserFunctions", linkAccountLog.SPName);
        }

        [Fact]
        public void ActionId_Always_ReturnsCorrectValue()
        {
            // Act
            var linkAccountLog = new UpdateFunctionsLog(1, 2, "127.0.0.1", Enumerable.Empty<Function>(), Enumerable.Empty<Function>());

            // Assert
            Assert.Equal(LogAction.UpdateChatbotFunctions, linkAccountLog.ActionId);
        }
    }
}