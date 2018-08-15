namespace Chatbot.Schedule.Service.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Quartz;

    internal class JobFailureHandler : IJobListener
    {
        public string Name => "FailJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            if (!context.JobDetail.JobDataMap.Contains(Constant.NumberOfRetriesKey))
            {
                context.JobDetail.JobDataMap.Put(Constant.NumberOfRetriesKey, 0);
            }

            var numberRetries = context.JobDetail.JobDataMap.GetIntValue(Constant.NumberOfRetriesKey);
            context.JobDetail.JobDataMap.Put(Constant.NumberOfRetriesKey, ++numberRetries);

            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            if (jobException == null)
            {
                return Task.CompletedTask;
            }

            var numberRetries = context.JobDetail.JobDataMap.GetIntValue(Constant.NumberOfRetriesKey);

            if (numberRetries <= 5)
            {
                var trigger = TriggerBuilder
                  .Create()
                  .WithIdentity(context.Trigger.Key)
                  .StartAt(DateTime.Now.AddSeconds(60 * numberRetries))
                  .WithSimpleSchedule(x => x.WithRepeatCount(0))
                  .ForJob(context.JobDetail)
                  .Build();

                context.JobDetail.JobDataMap.Put(Constant.NumberOfRetriesKey, numberRetries);

                Console.WriteLine(String.Format("Job with ID and type: {0}, {1} has thrown the exception: {2}. Running again in {3} seconds.",
                    context.JobDetail.Key, context.JobDetail.JobType, jobException, 60 * numberRetries));
                context.Scheduler.UnscheduleJob(context.Trigger.Key);
                context.Scheduler.ScheduleJob(context.JobDetail, trigger);
            }
            else
            {
                Console.WriteLine(String.Format("Job with ID and type: {0}, {1} has run {2} times and has failed each time.",
                  context.JobDetail.Key, context.JobDetail.JobType, 5));
                // Write more log here
            }

            return Task.CompletedTask;
        }
    }
}