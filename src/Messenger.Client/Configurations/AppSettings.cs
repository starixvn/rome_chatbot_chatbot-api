namespace Messenger.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public interface IMessengerSettings
    {
        string ApiEndpoint { get; }

        string ApiKey { get; }

        string SecureKey { get; }

        string ChatKey { get; }

        int BotAccountId { get; }

        string BotUserId { get; }
    }

    public class LetstalkSettings : IMessengerSettings
    {
        public LetstalkSettings(
            string apiEndpoint,
            string apiKey,
            string secureKey,
            string chatKey,
            int botAccountId,
            string botUserId)
        {
            ApiEndpoint = apiEndpoint;
            ApiKey = apiKey;
            SecureKey = secureKey;
            ChatKey = chatKey;
            BotAccountId = botAccountId;
            BotUserId = botUserId;
        }

        public string ApiEndpoint { get; }

        public string ApiKey { get; }

        public string SecureKey { get; }

        public string ChatKey { get; }

        public int BotAccountId { get; }

        public string BotUserId { get; }
    }

    public static class AppSettings
    {
        static AppSettings()
        {
            var botId = Configs["MessengerAPIClient:BotAccountId"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:BotAccountId");
            var messengerBotAccountId = int.Parse(botId);
            var messengerApiEndpoint = Configs["MessengerAPIClient:ApiEndpoint"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiEndpoint");
            var messengerApiKey = Configs["MessengerAPIClient:ApiKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiKey");
            var messengerSecureKey = Configs["MessengerAPIClient:SecureKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:SecureKey");
            var messengerChatKey = Configs["MessengerAPIClient:ChatKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ChatKey");
            var messengerBotUserId = Configs["MessengerAPIClient:BotUserId"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:BotUserId");

            Messenger = new LetstalkSettings(
                messengerApiEndpoint,
                messengerApiKey,
                messengerSecureKey,
                messengerChatKey,
                messengerBotAccountId,
                messengerBotUserId);
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IMessengerSettings Messenger { get; private set; }
    }
}