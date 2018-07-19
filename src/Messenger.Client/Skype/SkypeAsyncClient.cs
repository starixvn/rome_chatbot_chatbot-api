namespace Messenger.Client.Skype
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial class SkypeClient
    {
        public Task<Result> AddContactToTagAsync(string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> AddTagAsync(string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> DeleteTagAsync(string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Contact>> GetContactsAsync(string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Contact> GetProfileAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTagsAsync(int botId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserPublicKeyAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushToTagAsync(string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> RemoveContactFromTagAsync(string[] users, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetHashKeyAsync(string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetUserKeyAsync(string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWebhookAsync(string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWelcomeMessageAsync(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}