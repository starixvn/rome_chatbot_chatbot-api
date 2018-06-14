namespace Messenger.Client
{
    using System.Collections.Generic;
    using Messenger.Client.Models;

    public partial interface IMessengerClient
    {
        Result Push(string botName, IEnumerable<string> receivers, Payload payload);

        Result PushToTag(string botName, string tag, Payload payload);

        Result UpdateTag(string botName, string tag);

        Result DeleteTag(string botName, string tag);

        IEnumerable<string> GetTags(string botName);

        IEnumerable<Contact> GetContacts(string botName, string tag);

        Contact GetContact(string botName, string userId);

        Result AddContactToTag(string botName, string userId, string tag);

        Result RemoveContactFromTag(string botName, string userId, string tag);

        Result SetWebhook(string botName, string url);

        Result SetWelcomeMessage(string botName, string message);

        string GetContactPublicKey(string botName, string userId);

        Result SetContactKey(string botName, string userId, string hashId, string hashKey);

        Result SetHashKey(string botName, string hashId, string hashKey);
    }
}