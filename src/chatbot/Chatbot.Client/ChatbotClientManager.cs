namespace Chatbot.Client
{
    using System;
    using Chatbot.Client.Configurations;
    using Chatbot.Client.Helpers;

    public class ChatbotClientManager
    {
        private IBotClientConfigs botClientConfigs;
        private IRestClient restClient;

        private static ChatbotClientManager CurrentInstance = new ChatbotClientManager();

        public static ChatbotClientManager UseConfig(IBotClientConfigs botClientConfigs)
        {
            CurrentInstance.botClientConfigs = botClientConfigs ?? throw new ArgumentNullException(nameof(botClientConfigs));

            return CurrentInstance;
        }

        public ChatbotClientManager UseRestClient(IRestClient restClient)
        {
            CurrentInstance.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));

            return CurrentInstance;
        }

        public static IBotClientConfigs BotClientConfigs
            => CurrentInstance.botClientConfigs ?? throw new InvalidOperationException(nameof(CurrentInstance.botClientConfigs));

        public static IRestClient RestClient
            => CurrentInstance.restClient ?? throw new InvalidOperationException(nameof(CurrentInstance.restClient));
    }
}