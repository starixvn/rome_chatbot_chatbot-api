namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class ResetActionCountTimesCommandTests
    {
        [Theory]
        [InlineData(0, 1, 1, "name")]
        [InlineData(1, 0, 1, "name")]
        [InlineData(1, 1, 0, "name")]
        [InlineData(1, 1, 1, "")]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int userId, int actionId, int messengerType, string messengerUsername)
        {
            var result = new ResetActionCountTimesCommand(
                userId,
                actionId,
                messengerType,
                messengerUsername).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 1, 1, "name")]
        public void IsValid_Always_ReturnTrue(int userId, int actionId, int messengerType, string messengerUsername)
        {
            var result = new ResetActionCountTimesCommand(
                userId,
                actionId,
                messengerType,
                messengerUsername).IsValid();

            Assert.True(result);
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var command = new ResetActionCountTimesCommand(0, 0, 1, string.Empty);

            // Assert
            Assert.Equal("ResetUserActionCount", command.GetSettingKey());
        }
    }
}