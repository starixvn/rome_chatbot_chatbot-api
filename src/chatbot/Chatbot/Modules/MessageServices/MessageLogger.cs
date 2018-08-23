namespace Chatbot.Modules.MessageSenders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Helpers;
    using Chatbot.Models;
    using Chatbot.Modules.MessageServices.Params.Db;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Fanex.Data.Repository;

    public interface IMessageLogger
    {
        Task LogMessage(int sender, int receiver, Messenger messenger, string message, int status = 0, int functionId = 0);

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
            fromDate = minDate > fromDate ? minDate : fromDate;

            var criteria = new GetMessageLogCriteria(userId, fromDate, toDate, functionId, messengerType);

            return await dynamicRepository.FetchAsync<Message>(criteria);
        }

        public async Task LogMessage(
            int sender,
            int receiver,
            Messenger messenger,
            string message,
            int status = 0,
            int functionId = 0)
        {
            var command = new InsertMessageCommand(
                sender,
                receiver,
                message,
                messenger.Type,
                messenger.Username,
                status,
                functionId);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}