namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    public class UpdateSelfdestructionPeriodCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new UpdateSelfdestructionPeriodCommand(1, 15);

            // Assert
            Assert.Equal("Chatbot_UpdateSelfdestructionPeriod", command.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Act
            var command = new UpdateSelfdestructionPeriodCommand(userId, 15);

            // Assert
            Assert.False(command.IsValid());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidMinutes_ReturnsFalse(int minutes)
        {
            // Act
            var command = new UpdateSelfdestructionPeriodCommand(1, minutes);

            // Assert
            Assert.False(command.IsValid());
        }

        [Fact]
        public void IsValid_ValidCommand_ReturnsTrue()
        {
            // Act
            var command = new UpdateSelfdestructionPeriodCommand(1, 15);

            // Assert
            Assert.True(command.IsValid());
        }
    }
}