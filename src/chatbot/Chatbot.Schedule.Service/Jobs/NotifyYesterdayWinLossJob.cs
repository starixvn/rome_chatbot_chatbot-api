namespace Chatbot.Schedule.Service.Jobs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Quartz;

    internal class NotifyYesterdayWinLossJob : IJob
    {
        private readonly IMessageSender messageSender;

        public NotifyYesterdayWinLossJob(IMessageSender messageSender)
        {
            this.messageSender = messageSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var customers = GetCustomersEnableYesterdayWinLoss();

            foreach (var customer in customers)
            {
                var report = GetYesterdayWinLossReport(customer.Id);

                await messageSender.SendMessage(customer.Id, report);
            }
        }

        private IEnumerable<Customer> GetCustomersEnableYesterdayWinLoss()
        {
            return new List<Customer>();
        }

        private string GetYesterdayWinLossReport(string customerId)
        {
            return null;
        }
    }
}