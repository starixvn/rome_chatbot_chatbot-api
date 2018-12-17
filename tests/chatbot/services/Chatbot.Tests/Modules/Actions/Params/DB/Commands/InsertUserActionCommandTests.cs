namespace Chatbot.Tests.Modules.Actions.Params.DB.Commands
{
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Xunit;

    public class InsertUserActionCommandTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new InsertUserActionCommand(0, 0, string.Empty, string.Empty);

            // Assert
            Assert.Equal("Chatbot_InsertUserAction", command.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Act
            var command = new InsertUserActionCommand(userId, 1, string.Empty, string.Empty);

            // Assert
            Assert.False(command.IsValid());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidActionId_ReturnsFalse(int actionId)
        {
            // Act
            var command = new InsertUserActionCommand(1, actionId, string.Empty, string.Empty);

            // Assert
            Assert.False(command.IsValid());
        }

        [Fact]
        public void IsValid_ValidCommand_ReturnsTrue()
        {
            // Act
            var command = new InsertUserActionCommand(1, 1, string.Empty, string.Empty);

            // Assert
            Assert.True(command.IsValid());
        }
    }
}