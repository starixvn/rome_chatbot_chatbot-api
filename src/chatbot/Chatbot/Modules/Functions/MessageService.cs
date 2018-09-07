namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Data.Repository;

    public interface MessageService
    {
        Task<DateTime> GetLastQueryTime(int userId, int functionId, int messageType);

        Task UpdateLastQueryTime(int userId, int functionId, int messageType, DateTime lastQueryTime);
    }

    public class MessageServiceImpl : MessageService
    {
        private readonly IDynamicRepository dynamicRepository;

        public MessageServiceImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<DateTime> GetLastQueryTime(int userId, int functionId, int messageType)
        {
            var criteria = new GetMessageLastQueryTimeCriteria(userId, functionId, messageType);

            var result = await dynamicRepository.GetAsync<DateTime>(criteria).ConfigureAwait(false);

            if (result == DateTime.MinValue)
            {
                return DateTime.Now;
            }

            return result;
        }

        public Task UpdateLastQueryTime(int userId, int functionId, int messageType, DateTime lastQueryTime)
        {
            var command = new UpdateMessageLastQueryTimeCommand(userId, functionId, messageType, lastQueryTime);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}