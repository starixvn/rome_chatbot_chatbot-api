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
        Task<Result> SendMessage(int userId, string message, int functionId = 0);

        Task<Result> SendByFunction(int functionId, int userId);

        Task<List<Message>> GetMessageLogs(int userId, DateTimeOffset fromDate, DateTimeOffset toDate, int functionId = 0, int messengerType = 0);
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

        public Task<Result> SendByFunction(int functionId, int userId)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functionId), functionId);

            return restClient.ExecutePostAsync<Result>(
               $"message/sendByFunction",
               new
               {
                   userId,
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
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(fromDate), fromDate);
            ValidateInputParam(nameof(toDate), toDate);

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
    }
}