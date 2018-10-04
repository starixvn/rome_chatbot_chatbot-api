namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    public class InsertMessageCommandTests
    {
        [Theory]
        [InlineData(0, 0, "message")]
        [InlineData(1, 0, "")]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int sender, int receiver, string message)
        {
            var result = new InsertMessageCommand(
                sender,
                receiver,
                message,
                1,
                "name",
                1,
                1).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 0, "message")]
        [InlineData(0, 1, "dad")]
        [InlineData(1, 1, "dad")]
        [InlineData(1, -1, "dad")]
        public void IsValid_Always_ReturnTrue(int sender, int receiver, string message)
        {
            var result = new InsertMessageCommand(
                sender,
                receiver,
                message,
                1,
                "name",
                1,
                1).IsValid();

            Assert.True(result);
        }
    }
}