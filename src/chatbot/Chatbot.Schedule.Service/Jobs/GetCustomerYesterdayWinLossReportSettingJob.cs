namespace Chatbot.Schedule.Service.Jobs
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Quartz;
    using Quartz.Impl;

    internal class GetCustomerYesterdayWinLossReportSettingJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var factory = new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });
            var scheduler = await factory.GetScheduler();

            var customers = GetCustomersEnableYesterdayWinLoss();

            foreach (var customer in customers)
            {
                var job = JobBuilder.Create<GetAndSendCustomerReportJob>()
                   .WithIdentity($"YesterdayWinLoss-{customer.Id}", "YesterdayWinLoss")
                   .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"YesterdayWinLossTrigger-{customer.Id}", "YesterdayWinLoss")
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
        }

        private IEnumerable<Customer> GetCustomersEnableYesterdayWinLoss()
        {
            return new List<Customer>();
        }
    }
}