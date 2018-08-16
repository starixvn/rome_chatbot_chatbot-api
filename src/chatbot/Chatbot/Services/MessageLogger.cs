namespace Chatbot.Services
{
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Params.Db.Commands;
    using Fanex.Data.Repository;

    public interface IMessageLogger
    {
        Task LogMessage(int sender, int receiver, Messenger messenger, string message, int status = 0, int functionId = 0);
    }

    public class MessageLogger : IMessageLogger
    {
        private readonly IDynamicRepository dynamicRepository;

        public MessageLogger(IDynamicRepository dynamicRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public async Task LogMessage(
            int sender,
            int receiver,
            Messenger messenger,
            string message,
            int status = 0,
            int functionId = 0)
        {
            var command = new InsertMessageCommand(
                sender,
                receiver,
                message,
                messenger.Type,
                messenger.Username,
                status,
                functionId);

            await dynamicRepository.ExecuteAsync(command);
        }
    }
}