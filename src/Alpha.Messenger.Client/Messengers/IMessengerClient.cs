namespace Alpha.Messenger.Client.Messengers
{
    using System.Collections.Generic;
    using Alpha.Messenger.Client.Models;

    public interface IMessengerClient
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
    }
}