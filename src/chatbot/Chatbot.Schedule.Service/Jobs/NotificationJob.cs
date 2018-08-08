namespace Chatbot.Schedule.Service.Jobs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Chatbot.Schedule.Service.MessageHandler;
    using Chatbot.Schedule.Service.MessageHandler.Builders;
    using Quartz;

    internal class NotificationJob : IJob
    {
        private readonly IMessageSender messageSender;
        private readonly SettingService settingService;
        private readonly IMessageBuilderFactory messageBuilderFactory;

        public NotificationJob() : this(null, null, null)
        {
        }

        public NotificationJob(
            IMessageSender messageSender,
            SettingService settingService,
            IMessageBuilderFactory messageBuilderFactory)
        {
            this.messageSender = messageSender ?? new MessageSender();
            this.settingService = settingService ?? new SettingServiceImpl();
            this.messageBuilderFactory = messageBuilderFactory ?? new MessageBuilderFactory();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var functionId = dataMap.GetIntValue("functionId");
            var users = await settingService.GetUsersByFunction(functionId, 0);

            Parallel.ForEach(users, async (user) =>
            {
                var messageBuilder = messageBuilderFactory.CreateMessageBuilder(functionId);

                var message = messageBuilder.GetMessage(new Dictionary<string, object>
                {
                    { "custId", user.CustId },
                    { "language", user.Language }
                });

                await messageSender.SendMessage(user.Id, message);
            });
        }
    }
}