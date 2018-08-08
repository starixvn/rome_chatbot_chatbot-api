namespace Chatbot.Client.Services
{
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface MessageService
    {
        Task<Result> SendMessage(int userId, string message, int functionId = 0);
    }

    public class MessageServiceImpl : ServiceBase, MessageService
    {
        private readonly IRestClient restClient;

        public MessageServiceImpl() : this(null)
        {
        }

        internal MessageServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<Result> SendMessage(int userId, string message, int functionId = 0)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(message), message);

            return restClient.ExecutePostAsync<Result>(
                $"message/send",
                new
                {
                    userId,
                    message,
                    functionId
                });
        }
    }
}