namespace Chatbot.Services
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Models;

    public interface IMessageSender
    {
        Task<Result> SendMessage(Messenger messenger, string message);

        Task<bool> IsMessengerAccountNotExisted(Messenger messenger);
    }

    public class MessageSender : IMessageSender
    {
        private readonly MessengerFactory messengerFactory;

        public MessageSender(MessengerFactory messengerFactory)
        {
            this.messengerFactory = messengerFactory;
        }

        public async Task<Result> SendMessage(Messenger messenger, string message)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var result = await client.PushAsync(messenger.Username, message);

            if (result.IsNotOk)
            {
                throw new InvalidOperationException(result.Message);
            }

            return new Result(result.Status, result.Message);
        }

        /// <summary>
        /// Violate Single responsibility
        /// </summary>
        public async Task<bool> IsMessengerAccountNotExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);

            var profileResult = await client.GetProfileAsync(messenger.Username);

            return !profileResult.Item2;
        }
    }
}