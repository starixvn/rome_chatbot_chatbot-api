namespace Chatbot.Schedule.Service
{
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Quartz;

    public interface IMessageSender
    {
        Task SendMessage(string customerId, string message);
    }

    public class MessageSender : IMessageSender
    {
        private readonly IJobRunner _jobRunner;

        public MessageSender(IJobRunner jobRunner)
        {
            _jobRunner = jobRunner;
        }

        public async Task SendMessage(string customerId, string message)
        {
            await _jobRunner.Start(
                    new ScheduleFunction { Name = "SendMessage" },
                    trigger: TriggerBuilder
                        .Create()
                        .StartNow()
                        .UsingJobData("customerId", customerId)
                        .UsingJobData("message", message)
                        .Build());
        }
    }
}