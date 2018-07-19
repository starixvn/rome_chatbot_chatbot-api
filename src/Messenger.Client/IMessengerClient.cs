namespace Messenger.Client
{
    using System.Collections.Generic;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Result Push(IEnumerable<string> receivers, Payload payload);

        Result Push(IEnumerable<string> receivers, string message);

        Result PushToTag(string tag, Payload payload);

        Result AddTag(string tag);

        Result DeleteTag(string tag);

        IEnumerable<string> GetTags(int botId);

        IEnumerable<Contact> GetContacts(string tag);

        Contact GetProfile(string userId);

        Result AddContactToTag(string[] users, string tag);

        Result RemoveContactFromTag(string[] users, string tag);

        Result SetWebhook(string url);

        Result SetWelcomeMessage(string message);

        Result ChangeDisplayName( string botName);

        string GetUserPublicKey(string userId);

        Result SetUserKey(string userId, string hashId, string hashKey);

        Result SetHashKey(string hashId, string hashKey);
    }
}