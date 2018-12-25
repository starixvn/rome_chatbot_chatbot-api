namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    public class UpdateSettingCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new UpdateSettingCommand(string.Empty, string.Empty);

            // Assert
            Assert.Equal("UpdateSetting", command.GetSettingKey());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_InvalidSettingName_ReturnsFalse(string settingName)
        {
            // Act
            var command = new UpdateSettingCommand(settingName, string.Empty);

            // Assert
            Assert.False(command.IsValid());
        }

        [Fact]
        public void IsValid_ValidCommand_ReturnsFalse()
        {
            // Act
            var command = new UpdateSettingCommand("setting name", string.Empty);

            // Assert
            Assert.True(command.IsValid());
        }
    }
}