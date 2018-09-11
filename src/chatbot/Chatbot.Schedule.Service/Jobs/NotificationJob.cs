namespace Chatbot.Schedule.Service.Jobs
{
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Constants;
    using Chatbot.Schedule.Service.MessageHandler;
    using Quartz;

    public class NotificationJob : IJob
    {
        private readonly IMessageSender messageSender;
        private readonly MessageService messageService;

        public NotificationJob() : this(null, null)
        {
        }

        public NotificationJob(
            IMessageSender messageSender,
            MessageService messageService)
        {
            this.messageSender = messageSender ?? new MessageSender();
            this.messageService = messageService ?? new MessageServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var functionId = dataMap.GetIntValue(JobDataKey.FunctionId);

                var functionMessages = await messageService.GetFunctionMessages(functionId);

                Parallel.ForEach(functionMessages, async (functionMessage) =>
                {
                    await messageSender.SendMessageToUser(functionMessage.UserId, functionId, functionMessage.Message);
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