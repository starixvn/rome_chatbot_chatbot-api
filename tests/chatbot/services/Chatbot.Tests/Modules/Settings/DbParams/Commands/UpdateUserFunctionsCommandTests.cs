namespace Chatbot.Tests.Modules.Settings.DbParams.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class UpdateUserFunctionsCommandTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_ParamsIsNegativeOrZero_ReturnFalse(int userId)
        {
            var result = new UpdateUserFunctionsCommand(
                userId,
                new List<Function> { new Function() })
                .IsValid();

            Assert.False(result);
        }

        [Fact]
        public void IsValid_FunctionListIsEmpty_ReturnFalse()
        {
            var result = new UpdateUserFunctionsCommand(
                1,
                Enumerable.Empty<Function>())
                .IsValid();

            Assert.False(result);
        }

        [Fact]
        public void FunctionXml_Always_ReturnFormattedXml()
        {
            var functionList = new List<Function>
            {
                new Function
                {
                    Id = 1,
                    Command = "cm1",
                    Enabled = true
                },
                new Function
                {
                    Id = 2,
                    Command = "cm2",
                    Enabled = false
                }
            };
            var actualFormattedXml = new UpdateUserFunctionsCommand(
                1,
                functionList).FunctionXml;

            Assert.Equal("<ROOT><Function UserId=\"1\" FunctionId=\"1\" Command=\"cm1\" Enable=\"True\"/> <Function UserId=\"1\" FunctionId=\"2\" Command=\"cm2\" Enable=\"False\"/> </ROOT>", actualFormattedXml);
        }
    }
}