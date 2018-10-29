namespace Chatbot.Client.Services
{
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface CommandService
    {
        Task<Result> Run(string command, string parameter, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : CommandService
    {
        private readonly IRestClient restClient;

        public CommandServiceImpl() : this(null)
        {
        }

        internal CommandServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<Result> Run(string command, string parameter, int messengerType, string messengerUsername)
        {
            InputValidator.ValidateInputParam(nameof(command), command);
            InputValidator.ValidateInputParam(nameof(messengerUsername), messengerUsername);
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>(
                $"command/run",
                new
                {
                    command,
                    parameter,
                    messengerUsername,
                    messengerType
                });
        }
    }
}