namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    public class UpdateUserLanguageCommandTests
    {
        [Theory]
        [InlineData(1, "")]
        [InlineData(0, "en-us")]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int userId, string language)
        {
            var result = new UpdateUserLanguageCommand(
                userId,
                language).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, "en-us")]
        public void IsValid_Always_ReturnTrue(int userId, string language)
        {
            var result = new UpdateUserLanguageCommand(
                userId,
                language).IsValid();

            Assert.True(result);
        }
    }
}