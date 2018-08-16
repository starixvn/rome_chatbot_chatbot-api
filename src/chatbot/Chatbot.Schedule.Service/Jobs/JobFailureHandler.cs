namespace Chatbot.Schedule.Service.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Quartz;
    using Serilog;

    internal class JobFailureHandler : IJobListener
    {
        private const string NumberOfRetriesKey = "NumberOfRetries";
        private const int MaxRetries = 5;
        private const int RetryInterval = 60;

        public string Name => "FailJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            if (!context.JobDetail.JobDataMap.Contains(NumberOfRetriesKey))
            {
                context.JobDetail.JobDataMap.Put(NumberOfRetriesKey, 0);
            }

            var numberRetries = context.JobDetail.JobDataMap.GetIntValue(NumberOfRetriesKey);
            context.JobDetail.JobDataMap.Put(NumberOfRetriesKey, ++numberRetries);

            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            if (jobException == null)
            {
                return Task.CompletedTask;
            }

            var numberRetries = context.JobDetail.JobDataMap.GetIntValue(NumberOfRetriesKey);

            if (numberRetries <= MaxRetries)
            {
                var trigger = TriggerBuilder
                  .Create()
                  .WithIdentity(context.Trigger.Key)
                  .StartAt(DateTime.Now.AddSeconds(RetryInterval * numberRetries))
                  .WithSimpleSchedule(x => x.WithRepeatCount(0))
                  .ForJob(context.JobDetail)
                  .Build();

                context.JobDetail.JobDataMap.Put(NumberOfRetriesKey, numberRetries);

                Log.Logger.Error($"Job with ID and type: {context.JobDetail.Key}, {context.JobDetail.JobType}" +
                    $" has thrown the exception: {jobException}. Running again in {RetryInterval * numberRetries} seconds.");

                context.Scheduler.UnscheduleJob(context.Trigger.Key);
                context.Scheduler.ScheduleJob(context.JobDetail, trigger);
            }
            else
            {
                Log.Logger.Error($"Job with ID and type: {context.JobDetail.Key}, {context.JobDetail.JobType} has run {MaxRetries} times and has failed each time.");
                // Write more log here
            }

            return Task.CompletedTask;
        }
    }
}