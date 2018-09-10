namespace Chatbot.Client.Tests.Configurations
{
    using Chatbot.Client.Configurations;
    using Xunit;

    public class BotClientConfigsTests
    {
        [Fact]
        public void Ctor_Always_AssignPropertyValue()
        {
            var config = new BotClientConfigs("apiBaseUrl", "apiKey");

            Assert.Equal("apiBaseUrl", config.ApiBaseUrl);
            Assert.Equal("apiKey", config.ApiKey);
        }
    }
}