namespace Messenger.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Task<Result> PushAsync(string botName, IEnumerable<string> receivers, Payload payload);

        Task<Result> PushToTagAsync(string botName, string tag, Payload payload);

        Task<Result> UpdateTagAsync(string botName, string tag);

        Task<Result> DeleteTagAsync(string botName, string tag);

        Task<IEnumerable<string>> GetTagsAsync(string botName);

        Task<IEnumerable<Contact>> GetContactsAsync(string botName, string tag);

        Task<Contact> GetContactAsync(string botName, string userId);

        Task<Result> AddContactToTagAsync(string botName, string userId, string tag);

        Task<Result> RemoveContactFromTagAsync(string botName, string userId, string tag);

        Task<Result> SetWebhookAsync(string botName, string url);

        Task<Result> SetWelcomeMessageAsync(string botName, string message);

        Task<string> GetContactPublicKeyAsync(string botName, string userId);

        Task<Result> SetContactKeyAsync(string botName, string userId, string hashId, string hashKey);

        Task<Result> SetHashKeyAsync(string botName, string hashId, string hashKey);
    }
}