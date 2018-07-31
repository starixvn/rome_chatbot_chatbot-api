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
        /// <summary>
        /// can support other messengers here
        /// </summary>
        public IMessengerClient CreateMessengerClient(int messengerType)
        {
            switch (messengerType)
            {
                default:
                    return new LetstalkClient(AppSettings.Messenger);
            }
        }
    }
}