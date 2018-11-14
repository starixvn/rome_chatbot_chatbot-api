namespace Chatbot.Tests.Modules.Builders.Templates
{
    using Chatbot.Modules.MessageBuilders.Templates;
    using Xunit;

    [Trait("Modules", "Buildes/Templates")]
    public class LetstalkFormatterTests
    {
        [Fact]
        public void NewLine_Alway_ReturnNewLineCharacter()
        {
            var newLine = new LetstalkFormatter().NewLine();

            Assert.Equal("\n", newLine);
        }

        [Fact]
        public void Point_Alway_ReturnPointCharacter()
        {
            var point = new LetstalkFormatter().Point();

            Assert.Equal("\u2022", point);
        }
    }
}