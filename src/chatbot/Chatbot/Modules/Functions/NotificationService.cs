namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Data.Repository;

    public interface NotificationService
    {
        Task<DateTime> GetLastQueryTime(int functionId);

        Task UpdateLastQueryTime(int functionId, DateTime lastQueryTime);
    }

    public class NotificationServiceImpl : NotificationService
    {
        private readonly IDynamicRepository dynamicRepository;

        public NotificationServiceImpl(
            IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<DateTime> GetLastQueryTime(int functionId)
        {
            var criteria = new GetMessageLastQueryTimeCriteria(functionId);

            var result = await dynamicRepository.GetAsync<DateTime>(criteria).ConfigureAwait(false);

            if (result == DateTime.MinValue)
            {
                return DateTime.Now;
            }

            return result;
        }

        public Task UpdateLastQueryTime(int functionId, DateTime lastQueryTime)
        {
            var command = new UpdateMessageLastQueryTimeCommand(functionId, lastQueryTime);

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}