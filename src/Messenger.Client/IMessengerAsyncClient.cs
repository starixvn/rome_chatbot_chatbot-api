namespace Messenger.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Task<Result> PushAsync(int botId, IEnumerable<string> receivers, Payload payload);

        Task<Result> PushToTagAsync(int botId, string tag, Payload payload);

        Task<Result> AddTagAsync(int botId, string tag);

        Task<Result> DeleteTagAsync(int botId, string tag);

        Task<IEnumerable<string>> GetTagsAsync(int botId);

        Task<IEnumerable<Contact>> GetContactsAsync(int botId, string tag);

        Task<Contact> GetProfileAsync(int botId, string userId);

        Task<Result> AddContactToTagAsync(int botId, string[] users, string tag);

        Task<Result> RemoveContactFromTagAsync(int botId, string[] users, string tag);

        Task<Result> SetWebhookAsync(int botId, string url);

        Task<Result> SetWelcomeMessageAsync(int botId, string message);

        Task<string> GetUserPublicKeyAsync(int botId, string userId);

        Task<Result> SetUserKeyAsync(int botId, string userId, string hashId, string hashKey);

        Task<Result> SetHashKeyAsync(int botId, string hashId, string hashKey);
    }
}