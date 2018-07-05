namespace Messenger.Client.Skype
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Bot.Client;
    using Messenger.Client.Models;

    public partial class SkypeClient : IMessengerClient
    {
        private readonly IBotConnector botConnector;

        public SkypeClient(IBotConnector botConnector)
        {
            this.botConnector = botConnector;
        }

        public Result AddContactToTag(int botId, string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result AddTag(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result ChangeDisplayName(int botId, string botName)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> ChangeDisplayNameAsync(int botId, string botName)
        {
            throw new System.NotImplementedException();
        }

        public Result DeleteTag(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Contact> GetContacts(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Contact GetProfile(int botId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetTags(int botId)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserPublicKey(int botId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Result Push(int botId, IEnumerable<string> receivers, Payload payload)
        {
            var result = botConnector.Send(payload.HashContent, payload.UserId);

            return new Result { Status = 1, Message = result };
        }

        public Result Push(int botId, IEnumerable<string> receivers, string message)
        {
            throw new System.NotImplementedException();
        }

        public Result PushToTag(int botId, string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveContactFromTag(int botId, string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result SetHashKey(int botId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetUserKey(int botId, string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWebhook(int botId, string url)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWelcomeMessage(int botId, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}