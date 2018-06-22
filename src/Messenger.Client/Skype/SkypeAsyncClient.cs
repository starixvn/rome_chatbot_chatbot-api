namespace Messenger.Client.Skype
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial class SkypeClient
    {
        public Task<Result> AddContactToTagAsync(string botName, string userId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> DeleteTagAsync(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Contact> GetContactAsync(string botName, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetContactPublicKeyAsync(string botName, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Contact>> GetContactsAsync(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTagsAsync(string botName)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushAsync(string botName, IEnumerable<string> receivers, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> PushToTagAsync(string botName, string tag, Payload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> RemoveContactFromTagAsync(string botName, string userId, string tag)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetContactKeyAsync(string botName, string userId, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetHashKeyAsync(string botName, string hashId, string hashKey)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWebhookAsync(string botName, string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> SetWelcomeMessageAsync(string botName, string message)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> UpdateTagAsync(string botName, string tag)
        {
            throw new System.NotImplementedException();
        }
    }
}