namespace Chatbot.Schedule.Service.MessageHandler
{
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;

    public interface IMessageSender
    {
        Task SendMessage(int userId, string message);
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

        public async Task SendMessage(int userId, string message)
        {
            var scheduler = await schedulerFactory.GetScheduler();

            await scheduler.ScheduleJob(
                    JobBuilder.Create<SendMessageJob>()
                        .WithIdentity($"SendMessage{userId}", "SendMessage")
                        .UsingJobData("userId", userId)
                        .UsingJobData("message", message)
                        .Build(),
                    TriggerBuilder
                        .Create()
                        .StartNow()
                        .Build());
        }
    }
}