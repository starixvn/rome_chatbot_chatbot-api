namespace Chatbot.Client.Services
{
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface CommandService
    {
        Task<Result> Run(string command, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : ServiceBase, CommandService
    {
        private readonly IRestClient restClient;

        public CommandServiceImpl() : this(null)
        {
        }

        internal CommandServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<Result> Run(string command, int messengerType, string messengerUsername)
        {
            ValidateInputParam(nameof(command), command);
            ValidateInputParam(nameof(messengerUsername), messengerUsername);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>(
                $"run",
                new
                {
                    command,
                    messengerUsername,
                    messengerType
                });
        }
    }
}