namespace Chatbot.Modules.MessageSenders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Constants;
    using Chatbot.Models;
    using Fanex.Logging;

    public interface IMessageSender
    {
        Task<Result> SendMessage(Messenger messenger, string message, int functionId = 0);

        Task<Result> SendMessage(int userId, string message, IEnumerable<Messenger> messengers, int functionId = 0);

        Task<Result> SendMessage(int userId, Messenger messenger, string message, int functionId = 0);

        Task<bool> IsMessengerAccountNotExisted(Messenger messenger);
    }

    public class MessageSender : IMessageSender
    {
        private readonly MessengerFactory messengerFactory;
        private readonly IMessageLogger messageLogger;
        private readonly ILogger logger;

        public MessageSender(
            MessengerFactory messengerFactory,
            ILogger logger,
            IMessageLogger messageLogger)
        {
            this.messengerFactory = messengerFactory;
            this.messageLogger = messageLogger;
            this.logger = logger;
        }

        public async Task<Result> SendMessage(Messenger messenger, string message, int functionId = 0)
            => await SendMessage(0, messenger, message, functionId);

        public async Task<Result> SendMessage(int userId, Messenger messenger, string message, int functionId = 0)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var result = await client.PushAsync(messenger.Username, message);

            if (result.IsNotOk)
            {
                this.logger.Error(string.Join(
                    "\r\n",
                    $"Deliver message failed: {message}",
                    $"Code: {result.Status}, Message: {result.Message}"));
            }

            await messageLogger.LogMessage(
                    CommonId.BotUserId,
                    userId,
                    messenger,
                    message,
                    result.IsOk ? 1 : 0,
                    functionId);

            return new Result(result.IsOk ? 0 : result.Status, result.Message);
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

        public async Task<Result> SendMessage(int userId, string message, IEnumerable<Messenger> messengers, int functionId = 0)
        {
            var results = new List<Result>();

            foreach (var messenger in messengers)
            {
                results.Add(await SendMessage(userId, messenger, message, functionId));
            }

            return results.All(result => result.IsOk)
                ? Result.CreateSuccessfulResult()
                : new Result(1);
        }
    }
}