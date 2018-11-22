namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class InsertUserCommandTests
    {
        [Theory]
        [InlineData(0, 1, "en-us", 1)]
        [InlineData(1, 0, "", 1)]
        [InlineData(1, 1, "en-us", 0)]
        public void IsValid_ParamsIsNotValid_ReturnFalse(int userId, int subAccountId, string language, int custLevel)
        {
            var result = new InsertUserCommand(
                userId,
                subAccountId,
                language,
                custLevel).IsValid();

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, 1, "en-us", 1)]
        public void IsValid_Always_ReturnTrue(int userId, int subAccountId, string language, int custLevel)
        {
            var result = new InsertUserCommand(
                userId,
                subAccountId,
                language,
                custLevel).IsValid();

            Assert.True(result);
        }
    }
}
