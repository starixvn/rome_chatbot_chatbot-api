namespace Chatbot.Tests.Modules.Builders.Templates
{
    using Chatbot.Modules.MessageBuilders.Templates;
    using Xunit;

    public class LetstalkFormatterTests
    {
        [Fact]
        public void NewLine_Alway_ReturnNewLineCharacter()
        {
            var newLine = new LetstalkFormatter().NewLine();

            Assert.Equal("\n", newLine);
        }
    }
}