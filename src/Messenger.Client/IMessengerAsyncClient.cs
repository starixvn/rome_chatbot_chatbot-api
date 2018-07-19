namespace Messenger.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Task<Result> PushAsync(IEnumerable<string> receivers, Payload payload);

        Task<Result> PushToTagAsync(string tag, Payload payload);

        Task<Result> AddTagAsync(string tag);

        Task<Result> DeleteTagAsync(string tag);

        Task<IEnumerable<string>> GetTagsAsync(int botId);

        Task<IEnumerable<Contact>> GetContactsAsync(string tag);

        Task<Contact> GetProfileAsync(string userId);

        Task<Result> AddContactToTagAsync(string[] users, string tag);

        Task<Result> RemoveContactFromTagAsync(string[] users, string tag);

        Task<Result> SetWebhookAsync(string url);

        Task<Result> SetWelcomeMessageAsync(string message);

        Task<string> GetUserPublicKeyAsync(string userId);

        Task<Result> SetUserKeyAsync(string userId, string hashId, string hashKey);

        Task<Result> SetHashKeyAsync(string hashId, string hashKey);

        Task<Result> ChangeDisplayNameAsync(string botName);
    }
}