namespace Messenger.Client.Skype
{
    using System.Collections.Generic;
    using Fanex.Bot.Client;
    using Messenger.Client.Models;

    public partial class SkypeClient : IMessengerClient
    {
        private readonly IBotConnector botConnector;

        public SkypeClient(IBotConnector botConnector)
        {
            this.botConnector = botConnector;
        }

        public Result AddContactToTag(string botName, string userId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result DeleteTag(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Contact GetContact(string botName, string userId)
        {
            throw new System.NotImplementedException();
        }

        public string GetContactPublicKey(string botName, string userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Contact> GetContacts(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetTags(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Result Push(string botName, IEnumerable<string> receivers, Payload payload)
        {
            var result = botConnector.Send(payload.HashContent, payload.UserId);

            return new Result { Status = 1, Message = result };
        }

        public Result PushToTag(string botName, string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveContactFromTag(string botName, string userId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Result SetContactKey(string botName, string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetHashKey(string botName, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWebhook(string botName, string url)
        {
            throw new System.NotImplementedException();
        }

        public Result SetWelcomeMessage(string botName, string message)
        {
            throw new System.NotImplementedException();
        }

        public Result UpdateTag(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }
    }
}