namespace Alpha.Chatbot.Services
{
    using Messenger.Client;
    using Messenger.Client.Letstalk;

    public interface MessengerFactory
    {
        IMessengerClient CreateMessengerClient(int messagerType);
    }

    public class MessengerFactoryImpl : MessengerFactory
    {
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