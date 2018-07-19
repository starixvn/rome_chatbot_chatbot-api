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

        public Result AddContactToTag(string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result AddTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result ChangeDisplayName(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> ChangeDisplayNameAsync(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Result DeleteTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Contact> GetContacts(string tag)
        {
            throw new System.NotImplementedException();
        }

        public Contact GetProfile(string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetTags(int botId)
        {
            throw new System.NotImplementedException();
        }

        public string GetUserPublicKey(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Result Push(IEnumerable<string> receivers, Payload payload)
        {
            var result = botConnector.Send(payload.HashContent, payload.UserId);

            return new Result { Status = 1, Message = result };
        }

        public Result Push(IEnumerable<string> receivers, string message)
        {
            throw new System.NotImplementedException();
        }

        public Result PushToTag(string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveContactFromTag(string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result SetHashKey(string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetUserKey(string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWebhook(string url)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWelcomeMessage(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}