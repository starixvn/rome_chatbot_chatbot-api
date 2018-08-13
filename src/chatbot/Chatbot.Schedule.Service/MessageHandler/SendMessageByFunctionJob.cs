namespace Chatbot.Schedule.Service.MessageHandler
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Quartz;

    public class SendMessageByFunctionJob : IJob
    {
        private readonly MessageService messageService;

        public SendMessageByFunctionJob() : this(null)
        {
        }

        public SendMessageByFunctionJob(MessageService messageService)
        {
            this.messageService = messageService ?? new MessageServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var userId = dataMap.GetInt("userId");
            var functionId = dataMap.GetInt("functionId");

            try
            {
                var result = await messageService.SendByFunction(functionId, userId);

                if (result.IsNotOk)
                {
                    throw new JobExecutionException($"Send failed to user: {userId}");
                }
            }
            catch (Exception ex)
            {
                var jobExecutionException = new JobExecutionException(ex)
                {
                    RefireImmediately = true
                };

                throw jobExecutionException;
            }
        }
    }
}