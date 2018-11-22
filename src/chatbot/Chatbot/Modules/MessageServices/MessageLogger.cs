[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Chatbot.Tests")]

namespace Chatbot.Modules.MessageServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.MessageServices.Params.Db;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Helpers;
    using Fanex.Data.Repository;
    using MessengerClient = Messenger.Client;

    public interface IMessageLogger
    {
        Task LogMessage(int sender, int receiver, Messenger messenger, string message, int status = 0, int functionId = 0);

        Task LogMessage(int sender, int receiver, Messenger messenger, MessengerClient.Models.Message message, int status = 0, int functionId = 0);

        Task<IEnumerable<Message>> GetMessageLogs(int userId, DateTime fromDate, DateTime toDate, int functionId = 0, int messengerType = 0);
    }

    public class MessageLogger : IMessageLogger
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly Func<DateTime> getNow;

        public MessageLogger(
            IDynamicRepository dynamicRepository,
            IChatbotConfigs chatbotConfigs,
            Func<DateTime> getNow = null)
        {
            this.dynamicRepository = dynamicRepository;
            this.chatbotConfigs = chatbotConfigs;
            this.getNow = getNow ?? GetNow;
        }

        private static DateTime GetNow() => DateTime.Now;

        public async Task<IEnumerable<Message>> GetMessageLogs(int userId, DateTime fromDate, DateTime toDate, int functionId = 0, int messengerType = 0)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(fromDate), fromDate);
            InputValidator.Validate(nameof(toDate), toDate);

            var minDate = getNow().AddDays(-chatbotConfigs.MaxGetMessageLogDays);
            var actualFromDate = minDate > fromDate ? minDate : fromDate;

            var criteria = new GetMessageLogCriteria(userId, actualFromDate, toDate, functionId, messengerType);

            var messages = await dynamicRepository.FetchAsync<Message>(criteria).ConfigureAwait(false);

            return messages;
        }

        public async Task LogMessage(
            int sender,
            int receiver,
            Messenger messenger,
            string message,
            int status = 0,
            int functionId = 0)
        {
            await LogMessage(
                sender,
                receiver,
                messenger.Type,
                messenger.Username,
                message,
                status,
                functionId).ConfigureAwait(false);
        }

        public async Task LogMessage(int sender, int receiver, Messenger messenger, MessengerClient.Models.Message message, int status = 0, int functionId = 0)
        {
            var logMessage = new List<string>
            {
                message.Content
            };

            if (message.MessageMarkup != null)
            {
                foreach (var action in message.MessageMarkup.MessageActions)
                {
                    logMessage.Add($"<a href='#'>{action.Label}</a>");
                }
            }

            await LogMessage(
               sender,
               receiver,
               messenger.Type,
               messenger.Username,
               string.Join("<br>", logMessage),
               status,
               functionId).ConfigureAwait(false);
        }

        private async Task LogMessage(
            int sender,
            int receiver,
            int messengerType,
            string messengerUsername,
            string message,
            int status,
            int functionId)
        {
            var command = new InsertMessageCommand(
                sender,
                receiver,
                message,
                messengerType,
                messengerUsername,
                status,
                functionId);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}