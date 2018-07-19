namespace Alpha.Chatbot.Services
{
    using Messenger.Client;

    public interface MessengerFactory
    {
        IMessengerClient CreateMessengerClient(int messagerType);
    }

    public class MessengerFactoryImpl : MessengerFactory
    {
        /// <summary>
        /// can support other messengers here
        /// </summary>
        public IMessengerClient CreateMessengerClient(int messagerType)
        {
            switch (messagerType)
            {
                default:
                    return new LetstalkClient();
            }
        }
    }
}