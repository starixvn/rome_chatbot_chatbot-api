namespace Chatbot.Schedule.Service.MessageHandler
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Constants;
    using Chatbot.Schedule.Service.Jobs;
    using Quartz;
    using Quartz.Impl;

    public interface IMessageSender
    {
        Task SendMessageToUser(int userId, int functionId, string message);
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

        public async Task SendMessageToUser(int userId, int functionId, string message)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            scheduler.ListenerManager.AddJobListener(jobFailureHandler);

            var job = JobBuilder.Create<SendMessageToUserJob>()
                        .WithIdentity($"SendMessage_UserId:{userId}_FunctionId:{functionId}_Message:{message}_{Guid.NewGuid()}", "SendMessage")
                        .UsingJobData(JobDataKey.UserId, userId)
                        .UsingJobData(JobDataKey.FunctionId, functionId)
                        .UsingJobData(JobDataKey.Message, message)
                        .Build();
            var trigger = TriggerBuilder
                        .Create()
                        .StartNow()
                        .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}