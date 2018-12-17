namespace Chatbot.Tests.Modules.Actions.Params.DB.Commands
{
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Xunit;

    public class ResetUserActionCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new ResetUserActionCommand(0);

            // Assert
            Assert.Equal("Chatbot_ResetUserActionStatus", command.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Act
            var command = new ResetUserActionCommand(userId);

            // Assert
            Assert.False(command.IsValid());
        }

        [Fact]
        public void IsValid_ValidCommand_ReturnsTrue()
        {
            // Act
            var command = new ResetUserActionCommand(1);

            // Assert
            Assert.True(command.IsValid());
        }
    }
}