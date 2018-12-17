namespace Chatbot.Tests.Modules.Actions.Params.DB.Commands
{
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Xunit;

    public class UpdateUserActionCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new UpdateUserActionCommand(0, string.Empty, string.Empty);

            // Assert
            Assert.Equal("Chatbot_UpdateUserActionFullCommand", command.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidActionId_ReturnsFalse(int actionId)
        {
            // Act
            var command = new UpdateUserActionCommand(actionId, string.Empty, string.Empty);

            // Assert
            Assert.False(command.IsValid());
        }

        [Fact]
        public void IsValid_ValidCommand_ReturnsTrue()
        {
            // Act
            var command = new UpdateUserActionCommand(1, string.Empty, string.Empty);

            // Assert
            Assert.True(command.IsValid());
        }
    }
}