namespace Messenger.Client.Skype
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial class SkypeClient
    {
        public Task<Result> AddContactToTagAsync(int botId, string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> AddTagAsync(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> DeleteTagAsync(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Contact>> GetContactsAsync(int botId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Contact> GetProfileAsync(int botId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTagsAsync(int botId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserPublicKeyAsync(int botId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(int botId, IEnumerable<string> receivers, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushToTagAsync(int botId, string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> RemoveContactFromTagAsync(int botId, string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetHashKeyAsync(int botId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetUserKeyAsync(int botId, string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWebhookAsync(int botId, string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWelcomeMessageAsync(int botId, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}