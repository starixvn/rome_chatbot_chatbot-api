namespace Alpha.Chatbot.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

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