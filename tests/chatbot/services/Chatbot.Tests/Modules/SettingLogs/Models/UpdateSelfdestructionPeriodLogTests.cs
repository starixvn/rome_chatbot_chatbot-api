using Chatbot.Modules.SettingLogs.Constants;
using Chatbot.Modules.SettingLogs.Models;
using Xunit;

namespace Chatbot.Tests.Modules.SettingLogs.Models
{
    public class UpdateSelfdestructionPeriodLogTests
    {
        [Fact]
        public void SPName_Always_ReturnsCorrectValue()
        {
            // Act
            var model = new UpdateSelfdestructionPeriodLog(1, 2, "127.0.0.1", SelfdestructionPeriod.FifteenDays, SelfdestructionPeriod.FifteenMinutes);

            // Assert
            Assert.Equal("Chatbot_UpdateSelfdestructionPeriod", model.SPName);
        }

        [Fact]
        public void ActionId_Always_ReturnsCorrectValue()
        {
            // Act
            var model = new UpdateSelfdestructionPeriodLog(1, 2, "127.0.0.1", SelfdestructionPeriod.FifteenDays, SelfdestructionPeriod.FifteenMinutes);

            // Assert
            Assert.Equal(LogAction.UpdateSelfdestructionPeriod, model.ActionId);
        }
    }
}