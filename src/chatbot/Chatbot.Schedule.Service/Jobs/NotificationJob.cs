namespace Chatbot.Schedule.Service.Jobs
{
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Constants;
    using Chatbot.Schedule.Service.MessageHandler;
    using Quartz;

    public class NotificationJob : IJob
    {
        private readonly IMessageSender messageSender;
        private readonly SettingService settingService;

        public NotificationJob() : this(null, null)
        {
        }

        public NotificationJob(
            IMessageSender messageSender,
            SettingService settingService)
        {
            this.messageSender = messageSender ?? new MessageSender();
            this.settingService = settingService ?? new SettingServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var functionId = dataMap.GetIntValue(JobDataKey.FunctionId);
                var users = await settingService.GetUsersByFunction(functionId, 0);

                Parallel.ForEach(users, async (user) =>
                {
                    await messageSender.SendMessageByFunction(user.Id, functionId);
                });
            }
            catch (ChatbotClientException ex)
            {
                if (!ex.IsServiceNotAvailable)
                {
                    throw new JobExecutionException(ex.Message);
                }
            }
        }
    }
}