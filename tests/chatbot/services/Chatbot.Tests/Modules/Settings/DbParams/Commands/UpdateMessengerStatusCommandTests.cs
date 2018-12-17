namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class UpdateMessengerStatusCommandTests
    {
        [Theory]
        [InlineData(0, 1, "name")]
        [InlineData(1, 0, "name")]
        [InlineData(1, 1, "")]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int userId, int messengerType, string messengerUsername)
        {
            var result = new UpdateMessengerStatusCommand(
                userId,
                messengerType,
                messengerUsername,
                true).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 1, "name")]
        public void IsValid_Always_ReturnTrue(int userId, int messengerType, string messengerUsername)
        {
            var result = new UpdateMessengerStatusCommand(
                userId,
                messengerType,
                messengerUsername,
                false).IsValid();

            Assert.True(result);
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new UpdateMessengerStatusCommand(0, 0, string.Empty, false);

            // Assert
            Assert.Equal("UpdateMessengerAccountStatus", command.GetSettingKey());
        }
    }
}