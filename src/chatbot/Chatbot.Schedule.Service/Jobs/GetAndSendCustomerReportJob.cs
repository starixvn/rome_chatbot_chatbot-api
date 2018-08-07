namespace Chatbot.Schedule.Service.Jobs
{
    using System;
    using System.Threading.Tasks;
    using Quartz;

    internal class GetAndSendCustomerReportJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var customerId = dataMap.GetString("CustomerId");

            try
            {
                var report = GetCustomerReport(customerId);

                var isOk = SendReportMessage(customerId, report);

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

        private object GetCustomerReport(string customerId)
        {
            return null;
        }

        private bool SendReportMessage(string customerId, object report)
        {
            return true;
        }
    }
}