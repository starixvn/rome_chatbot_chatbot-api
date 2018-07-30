namespace Chatbot.Services
{
    using System;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Microsoft.Extensions.Configuration;

    public interface MessengerFactory
    {
        IMessengerClient CreateMessengerClient(int messengerType);
    }

    public class MessengerFactoryImpl : MessengerFactory
    {
        private readonly IConfiguration configuration;

        public MessengerFactoryImpl(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// can support other messengers here
        /// </summary>
        public IMessengerClient CreateMessengerClient(int messengerType)
        {
            switch (messengerType)
            {
                default:
                    return CreateLestalkClient();
            }
        }

        private LetstalkClient CreateLestalkClient()
        {
            var botId = configuration["MessengerAPIClient:BotAccountId"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:BotAccountId");
            var messengerBotAccountId = int.Parse(botId);
            var messengerApiEndpoint = configuration["MessengerAPIClient:ApiEndpoint"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiEndpoint");
            var messengerApiKey = configuration["MessengerAPIClient:ApiKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiKey");
            var messengerSecureKey = configuration["MessengerAPIClient:SecureKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:SecureKey");
            var messengerChatKey = configuration["MessengerAPIClient:ChatKey"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ChatKey");
            var messengerBotUserId = configuration["MessengerAPIClient:BotUserId"] ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:BotUserId");

            var letstalkSettings = new LetstalkSettings(
                messengerApiEndpoint,
                messengerApiKey,
                messengerSecureKey,
                messengerChatKey,
                messengerBotAccountId,
                messengerBotUserId);

            return new LetstalkClient(letstalkSettings);
        }
    }
}