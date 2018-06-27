namespace Messenger.Client
{
    using System.Collections.Generic;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Result Push(int botId, IEnumerable<string> receivers, Payload payload);

        Result PushToTag(int botId, string tag, Payload payload);

        Result AddTag(int botId, string tag);

        Result DeleteTag(int botId, string tag);

        IEnumerable<string> GetTags(int botId);

        IEnumerable<Contact> GetContacts(int botId, string tag);

        Contact GetProfile(int botId, string userId);

        Result AddContactToTag(int botId, string[] users, string tag);

        Result RemoveContactFromTag(int botId, string[] users, string tag);

        Result SetWebhook(int botId, string url);

        Result SetWelcomeMessage(int botId, string message);

        string GetUserPublicKey(int botId, string userId);

        Result SetUserKey(int botId, string userId, string hashId, string hashKey);

        Result SetHashKey(int botId, string hashId, string hashKey);
    }
}