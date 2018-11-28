namespace Chatbot.Schedule.Service.MessageHandler
{
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.Constants;
    using Quartz;

    public class SendMessageToUserJob : IJob
    {
        private readonly MessageService messageService;

        public SendMessageToUserJob() : this(null)
        {
        }

        public SendMessageToUserJob(MessageService messageService)
        {
            this.messageService = messageService ?? new MessageServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var userId = dataMap.GetInt(JobDataKey.UserId);
            var functionId = dataMap.GetInt(JobDataKey.FunctionId);
            var message = dataMap.GetString(JobDataKey.Message);
            var selfdestructionPeriod = dataMap.GetInt(JobDataKey.SelfdestructionPeriod);

            var result = await messageService.SendMessage(userId, message, selfdestructionPeriod, functionId);

            if (result.IsNotOk)
            {
                throw new JobExecutionException($"Send failed to user: {userId}");
            }
        }
    }
}