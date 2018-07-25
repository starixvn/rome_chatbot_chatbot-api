namespace Chatbot.Client.Sample
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using Chatbot.Client.Configurations;

    public static class AppSettings
    {
        static AppSettings()
        {
            var chatbotBaseUrl = Configs["Chatbot:BaseUrl"] ?? throw new InvalidOperationException("Missing config for Chatbot:BaseUrl");
            var chatbotApiKey = Configs["Chatbot:ApiKey"] ?? throw new InvalidOperationException("Missing config for Chatbot:ApiKey");

            BotClientConfigs = new BotClientConfigs(chatbotBaseUrl, chatbotApiKey);
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IBotClientConfigs BotClientConfigs { get; private set; }
    }
}