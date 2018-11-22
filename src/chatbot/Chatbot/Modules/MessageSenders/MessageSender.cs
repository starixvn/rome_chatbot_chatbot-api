namespace Chatbot.Modules.MessageSenders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using MessengerClient = Messenger.Client;

    public interface IMessageSender
    {
        Task<Result> SendMessage(Messenger messenger, string message, int functionId = 0, bool logToDatabase = true);

        Task<Result> SendMessage(int userId, string message, IEnumerable<Messenger> messengers, int functionId = 0, bool logToDatabase = true);

        Task<Result> SendMessage(int userId, Messenger messenger, string message, int functionId = 0, bool logToDatabase = true);

        Task<Result> SendMessage(int userId, Messenger messenger, MessengerClient.Models.Message message, int functionId = 0, bool logToDatabase = true);

        Task<bool> IsMessengerAccountNotExisted(Messenger messenger);
    }

    public class MessageSender : IMessageSender
    {
        private readonly MessengerFactory messengerFactory;
        private readonly IMessageLogger messageLogger;
        private readonly ILogger logger;
        private readonly IChatbotConfigs chatbotConfigs;

        public MessageSender(
            MessengerFactory messengerFactory,
            ILogger logger,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs)
        {
            this.messengerFactory = messengerFactory;
            this.messageLogger = messageLogger;
            this.logger = logger;
            this.chatbotConfigs = chatbotConfigs;
        }

        public async Task<Result> SendMessage(Messenger messenger, string message, int functionId = 0, bool logToDatabase = true)
            => await SendMessage(0, messenger, message, functionId).ConfigureAwait(false);

        public async Task<Result> SendMessage(int userId, Messenger messenger, string message, int functionId = 0, bool logToDatabase = true)
        {
            MessengerClient.Models.Result result;

            if (chatbotConfigs.DisabledSendToMessenger)
            {
                result = new MessengerClient.Models.Result { Status = 1 };
                var logMessage = string.Join(
                    "\r\n",
                    $"UserId: {userId}",
                    $"Messenger Username: {messenger.Username}",
                    $"Message: {message}");

                logger.Info(logMessage);
            }
            else
            {
                var client = messengerFactory.CreateMessengerClient(messenger.Type);
                result = await client.PushAsync(messenger.Username, message).ConfigureAwait(false);
            }

            if (result.IsNotOk)
            {
                var errorMessage = string.Join(
                    "\r\n",
                    $"Deliver message failed: {message}",
                    $"Code: {result.Status}, Message: {result.Message}");
                logger.Error(errorMessage);
            }

            if (logToDatabase)
            {
                await messageLogger.LogMessage(
                        CommonId.BotUserId,
                        userId,
                        messenger,
                        message,
                        result.IsOk ? 1 : 0,
                        functionId).ConfigureAwait(false);
            }

            return new Result(result.IsOk ? 0 : result.Status, result.Message);
        }

        /// <summary>
        /// Violate Single responsibility
        /// </summary>
        public async Task<bool> IsMessengerAccountNotExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);

            var profileResult = await client.GetProfileAsync(messenger.Username).ConfigureAwait(false);

            return !profileResult.Item2;
        }

        public async Task<Result> SendMessage(int userId, string message, IEnumerable<Messenger> messengers, int functionId = 0, bool logToDatabase = true)
        {
            var results = new List<Result>();

            foreach (var messenger in messengers)
            {
                results.Add(await SendMessage(userId, messenger, message, functionId, logToDatabase).ConfigureAwait(false));
            }

            return results.All(result => result.IsOk)
                ? Result.CreateSuccessfulResult()
                : new Result(1);
        }

        public async Task<Result> SendMessage(int userId, Messenger messenger, MessengerClient.Models.Message message, int functionId = 0, bool logToDatabase = true)
        {
            MessengerClient.Models.Result result;

            if (chatbotConfigs.DisabledSendToMessenger)
            {
                result = new MessengerClient.Models.Result { Status = 1 };
                var logMessage = string.Join(
                    "\r\n",
                    $"UserId: {userId}",
                    $"Messenger Username: {messenger.Username}",
                    $"Message: {JsonConvert.SerializeObject(message, Formatting.Indented)}");

                logger.Info(logMessage);
            }
            else
            {
                var client = messengerFactory.CreateMessengerClient(messenger.Type);

                result = await client.PushAsync(new List<string> { messenger.Username }, message).ConfigureAwait(false);
            }

            if (result.IsNotOk)
            {
                var errorMessage = string.Join(
                    "\r\n",
                    $"Deliver message failed: {message}",
                    $"Code: {result.Status}, Message: {result.Message}");
                logger.Error(errorMessage);
            }

            if (logToDatabase)
            {
                await messageLogger.LogMessage(
                        CommonId.BotUserId,
                        userId,
                        messenger,
                        message,
                        result.IsOk ? 1 : 0,
                        functionId).ConfigureAwait(false);
            }

            return new Result(result.IsOk ? 0 : result.Status, result.Message);
        }
    }
}