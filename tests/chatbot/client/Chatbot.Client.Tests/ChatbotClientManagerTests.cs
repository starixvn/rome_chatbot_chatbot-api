namespace Chatbot.Client.Tests
{
    using Chatbot.Client.Configurations;
    using Chatbot.Client.Helpers;
    using Xunit;

    public class ChatbotClientManagerTests
    {
        [Fact]
        public void UseConfig_Always_SetConfigs()
        {
            var config = new BotClientConfigs("apiBaseUrl", "apiKey");
            var restClient = NSubstitute.Substitute.For<IRestClient>();

            ChatbotClientManager
                .UseConfig(config)
                .UseRestClient(restClient);

            Assert.Equal(config.ApiBaseUrl, ChatbotClientManager.BotClientConfigs.ApiBaseUrl);
            Assert.Equal(config.ApiKey, ChatbotClientManager.BotClientConfigs.ApiKey);
        }
    }
}