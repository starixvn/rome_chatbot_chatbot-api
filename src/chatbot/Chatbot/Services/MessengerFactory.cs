namespace Chatbot.Services
{
    using Chatbot.Constants;
    using Messenger.Client;
    using Messenger.Client.Configurations;

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
                case MessengerType.Skype:
                    return new SkypeClient(AppSettings.Messenger);

                default:
                    return new LetstalkClient(AppSettings.Messenger);
            }
        }
    }
}