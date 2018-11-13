[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Chatbot.Tests")]

namespace Chatbot.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface MessageService
    {
        Task<Result> SendMessage(int userId, string message, int? selfdestructionPeriod, int functionId = 0);

        Task<List<Message>> GetMessageLogs(int userId, DateTimeOffset fromDate, DateTimeOffset toDate, int functionId = 0, int messengerType = 0);

        Task<IEnumerable<FunctionMessage>> GetFunctionMessages(int functionId);
    }

    public class MessageServiceImpl : MessageService
    {
        private readonly IRestClient restClient;

        public MessageServiceImpl() : this(null)
        {
        }

        internal MessageServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<Result> SendMessage(int userId, string message, int? selfdestructionPeriod, int functionId = 0)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(message), message);

            return restClient.ExecutePostAsync<Result>(
                $"message/send",
                new
                {
                    userId,
                    message,
                    selfdestructionPeriod,
                    functionId
                });
        }

        public Task<List<Message>> GetMessageLogs(
            int userId,
            DateTimeOffset fromDate,
            DateTimeOffset toDate,
            int functionId = 0,
            int messengerType = 0)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(fromDate), fromDate);
            InputValidator.ValidateInputParam(nameof(toDate), toDate);

            return restClient.ExecutePostAsync<List<Message>>(
               $"message/getMessageLogs",
               new
               {
                   userId,
                   fromDate = fromDate.ToUnixTimeMilliseconds(),
                   toDate = toDate.ToUnixTimeMilliseconds(),
                   functionId,
                   messengerType
               });
        }

        public Task<IEnumerable<FunctionMessage>> GetFunctionMessages(int functionId)
        {
            InputValidator.ValidateInputParam(nameof(functionId), functionId);

            return restClient.ExecuteGetAsync<IEnumerable<FunctionMessage>>(
               $"message/getFunctionMessages?functionId={functionId}");
        }
    }
}