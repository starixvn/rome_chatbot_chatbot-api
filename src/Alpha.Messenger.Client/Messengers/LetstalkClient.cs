namespace Alpha.Messenger.Client.Messengers
{
    using System.Collections.Generic;
    using Alpha.Messenger.Client.Helpers;
    using Alpha.Messenger.Client.Models;

    public class LetstalkClient : IMessengerClient
    {
        private readonly IRestClientHelper restClientHelper;

        public LetstalkClient(IRestClientHelper restClientHelper)
        {
            this.restClientHelper = restClientHelper;
        }

        public Result AddContactToTag(string botName, string userId, string tag)
            => restClientHelper.ExecutePost<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });

        public Result RemoveContactFromTag(string botName, string userId, string tag)
            => restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });

        public Result DeleteTag(string botName, string tag)
            => restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_tag/{botName}",
                new
                {
                    tag
                });

        public Contact GetContact(string botName, string userId)
        {
            // Need confirm
            throw new System.NotImplementedException();
        }

        public IEnumerable<Contact> GetContacts(string botName, string tag)
        {
            var result = restClientHelper.ExecuteGet<ContactListResult>($"bot/get_contacts/{botName}?tag={tag}");

            return result.Contacts;
        }

        public IEnumerable<string> GetTags(string botName)
        {
            var result = restClientHelper.ExecuteGet<TagListResult>($"bot/list_tag/{botName}");

            return result.Tags;
        }

        public Result Push(string botName, IEnumerable<string> receivers, Payload payload)
            => restClientHelper.ExecutePost<Result>(
                $"bot/push/{botName}",
                new
                {
                    receiver = receivers,
                    payload
                });

        public Result PushToTag(string botName, string tag, Payload payload)
            => restClientHelper.ExecutePost<Result>(
                $"bot/push_to_tag/{botName}",
                new
                {
                    payload,
                    tag
                });

        public Result SetWebhook(string botName, string url)
            => restClientHelper.ExecutePost<Result>(
                $"bot/set_webhook/{botName}",
                new
                {
                    url
                });

        public Result SetWelcomeMessage(string botName, string message)
            => restClientHelper.ExecutePost<Result>(
                $"bot/set_welcome_msg/{botName}",
                new
                {
                    message
                });

        public Result UpdateTag(string botName, string tag)
        {
            // Need confirm with onelab team
            throw new System.NotImplementedException();
        }
    }
}