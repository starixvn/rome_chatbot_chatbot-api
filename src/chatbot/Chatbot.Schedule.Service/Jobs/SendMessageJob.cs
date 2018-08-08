namespace Chatbot.Schedule.Service.Jobs
{
    using System;
    using System.Threading.Tasks;
    using Quartz;

    internal class SendMessageJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var customerId = dataMap.GetString("customerId");
            var message = dataMap.GetString("message");

            try
            {
                var isOk = SendMessage(customerId, message);

                if (!isOk)
                {
                    throw new JobExecutionException($"Send failed to customer: {customerId}");
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

            return Task.CompletedTask;
        }

        private bool SendMessage(string customerId, string message)
        {
            return true;
        }
    }
}