namespace Chatbot.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Constants;
    using Chatbot.Models;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using System;

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
        private readonly ILogger<MessageSender> logger;
        private readonly IMessageLogger messageLogger;

        public MessageSender(
            MessengerFactory messengerFactory,
            ILogger<MessageSender> logger,
            IMessageLogger messageLogger)
        {
            this.messengerFactory = messengerFactory;
            this.logger = logger;
            this.messageLogger = messageLogger;
        }

        public async Task<Result> SendMessage(Messenger messenger, string message, int functionId = 0)
            => await SendMessage(0, messenger, message, functionId);

        public async Task<Result> SendMessage(int userId, Messenger messenger, string message, int functionId = 0)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var result = await client.PushAsync(messenger.Username, message);

            if (result.IsNotOk)
            {
                this.logger.LogError(string.Join(
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
            try
            {
                var client = messengerFactory.CreateMessengerClient(messenger.Type);

                var profileResult = await client.GetProfileAsync(messenger.Username);

                return !profileResult.Item2;
            }
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
            }

            return await Task.FromResult(false);
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