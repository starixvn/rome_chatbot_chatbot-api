namespace Messenger.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Bot.Client;
    using Messenger.Client.Models;

    public class SkypeClient : IMessengerClient
    {
        private readonly IBotConnector botConnector;

        public SkypeClient(IBotConnector botConnector = null)
        {
            this.botConnector = botConnector ?? new BotConnector();
        }

        public Task<Result> ChangeDisplayNameAsync(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Task<(Contact, bool)> GetProfileAsync(string accountId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, Payload payload)
        {
            var result = botConnector.Send(payload.HashContent, payload.UserId);

            return Task.FromResult(new Result { Status = 1, Message = result });
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, string message)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(string accountId, string message)
        {
            var result = botConnector.Send(message, accountId);

            return Task.FromResult(new Result { Status = 1, Message = result });
        }

        public Task<Result> SetWebhookAsync(string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWelcomeMessageAsync(string message)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetAutoReplyAsync(string message, bool enabled, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }
    }
}