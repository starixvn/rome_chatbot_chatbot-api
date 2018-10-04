namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
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
        private readonly IDictionary<int, string> LastQueryFunctionSettingNameMapper;

        public NotificationServiceImpl(
            IDynamicRepository dynamicRepository,
            IChatbotConfigs chatbotConfigs)
        {
            this.dynamicRepository = dynamicRepository;
            LastQueryFunctionSettingNameMapper = new Dictionary<int, string>
            {
                { chatbotConfigs.PersonalMessageFunctionId, "LastQueryPersonalMessage" },
                { chatbotConfigs.SpecialAnnouncementFunctionId, "LastQuerySpecialAnnouncement" },
                { chatbotConfigs.NormalAnnouncementFunctionId, "LastQueryNormalAnnouncement" },
                { chatbotConfigs.SystemUpdateFunctionId, "LastQuerySystemAnnouncement" }
            };
        }

        public async Task<DateTime> GetLastQueryTime(int functionId)
        {
            var criteria = new GetSettingCriteria(LastQueryFunctionSettingNameMapper[functionId]);

            var result = await dynamicRepository.GetAsync<DateTime>(criteria).ConfigureAwait(false);

            if (result == DateTime.MinValue)
            {
                return DateTime.Now;
            }

            return result;
        }

        public Task UpdateLastQueryTime(int functionId, DateTime lastQueryTime)
        {
            var command = new UpdateSettingCommand(
                LastQueryFunctionSettingNameMapper[functionId],
                lastQueryTime.ToString(CultureInfo.InvariantCulture));

            return dynamicRepository.ExecuteAsync(command);
        }
    }
}