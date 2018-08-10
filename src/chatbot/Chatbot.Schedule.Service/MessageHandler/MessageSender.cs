namespace Chatbot.Schedule.Service.MessageHandler
{
    using System;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;

    public interface IMessageSender
    {
        Task SendMessageByFunction(int userId, int functionId);
    }

    public class MessageSender : IMessageSender
    {
        private readonly ISchedulerFactory schedulerFactory;

        public MessageSender() : this(null)
        {
        }

        public MessageSender(ISchedulerFactory schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory ??
                new StdSchedulerFactory(new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                });
        }

        public async Task SendMessageByFunction(int userId, int functionId)
        {
            var scheduler = await schedulerFactory.GetScheduler();

            await scheduler.ScheduleJob(
                    JobBuilder.Create<SendMessageByFunctionJob>()
                        .WithIdentity($"SendMessage_{userId}_{functionId}_{DateTime.UtcNow.ToString()}", "SendMessage")
                        .UsingJobData("userId", userId)
                        .UsingJobData("functionId", functionId)
                        .Build(),
                    TriggerBuilder
                        .Create()
                        .StartNow()
                        .Build());
        }
    }
}