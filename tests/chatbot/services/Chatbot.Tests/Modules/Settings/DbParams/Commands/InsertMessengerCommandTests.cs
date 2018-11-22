namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class InsertMessengerCommandTests
    {
        [Theory]
        [InlineData(0, 1, "name", "124578")]
        [InlineData(1, 0, "name", "124578")]
        [InlineData(1, 1, "name", "")]
        [InlineData(1, 1, "", "124578")]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int userId, int messengerType, string messengerUsername, string verificationCode)
        {
            var result = new InsertMessengerCommand(
                userId,
                messengerType,
                messengerUsername,
                verificationCode).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 1, "name", "124578")]
        public void IsValid_Always_ReturnTrue(int userId, int messengerType, string messengerUsername, string verificationCode)
        {
            var result = new InsertMessengerCommand(
                userId,
                messengerType,
                messengerUsername,
                verificationCode).IsValid();

            Assert.True(result);
        }
    }
}