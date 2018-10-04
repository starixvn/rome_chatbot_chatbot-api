namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

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
    }
}