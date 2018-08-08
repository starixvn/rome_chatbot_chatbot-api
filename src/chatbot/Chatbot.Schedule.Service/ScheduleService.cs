namespace Chatbot.Schedule.Service
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Microsoft.Extensions.Hosting;

    internal class ScheduleService : BackgroundService
    {
        private readonly IJobRunner jobRunner;

        public ScheduleService(IJobRunner jobRunner)
        {
            this.jobRunner = jobRunner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scheduleFunctions = GetScheduleFunctions();

            foreach (var function in scheduleFunctions)
            {
                await jobRunner.Start(function);
            }
        }

        private IEnumerable<ScheduleFunction> GetScheduleFunctions()
        {
            return new List<ScheduleFunction>
            {
                new ScheduleFunction {
                    Name = "NotifyYesterdayWinLoss",
                    GroupName = "Notification",
                    CronExpression = "0 0 16 ? * * *"
                }
            };
        }
    }
}