namespace Chatbot.Schedule.Service.MessageHandler
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Jobs;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;

    public interface IMessageSender
    {
        Task SendMessageByFunction(int userId, int functionId);
    }

    public class MessageSender : IMessageSender
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobListener jobFailureHandler;

        public MessageSender() : this(null, null)
        {
        }

        public MessageSender(ISchedulerFactory schedulerFactory, IJobListener jobFailureHandler)
        {
            this.schedulerFactory = schedulerFactory ??
                new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });

            this.jobFailureHandler = jobFailureHandler ?? new JobFailureHandler();
        }

        public async Task SendMessageByFunction(int userId, int functionId)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.ListenerManager.AddJobListener(jobFailureHandler);

            var job = JobBuilder.Create<SendMessageByFunctionJob>()
                        .WithIdentity($"SendMessage_{userId}_{functionId}_{DateTime.UtcNow}", "SendMessage")
                        .UsingJobData("userId", userId)
                        .UsingJobData("functionId", functionId)
                        .Build();
            var trigger = TriggerBuilder
                        .Create()
                        .StartNow()
                        .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}