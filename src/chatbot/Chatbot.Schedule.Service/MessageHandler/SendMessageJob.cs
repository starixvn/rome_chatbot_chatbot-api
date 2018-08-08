namespace Chatbot.Schedule.Service.MessageHandler
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using Quartz;

    public class SendMessageJob : IJob
    {
        private readonly MessageService messageService;

        public SendMessageJob() : this(null)
        {
        }

        public SendMessageJob(MessageService messageService)
        {
            this.messageService = messageService ?? new MessageServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var userId = dataMap.GetInt("userId");
            var message = dataMap.GetString("message");

            try
            {
                var result = await SendMessage(userId, message);

                if (!result.IsOk)
                {
                    throw new JobExecutionException($"Send failed to customer: {userId}");
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

        private async Task<Result> SendMessage(int userId, string message)
        {
            return await messageService.SendMessage(userId, message);
        }
    }
}