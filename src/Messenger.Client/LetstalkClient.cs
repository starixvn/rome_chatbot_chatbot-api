namespace Messenger.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;

    public partial class LetstalkClient : IMessengerClient
    {
        private readonly IRestClientHelper restClientHelper;

        public LetstalkClient(IRestClientHelper restClientHelper)
        {
            this.restClientHelper = restClientHelper;
        }

        public Result AddContactToTag(string botName, string userId, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePost<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });
        }

        public Result RemoveContactFromTag(string botName, string userId, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });
        }

        public Result DeleteTag(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_tag/{botName}",
                new
                {
                    tag
                });
        }

        public Contact GetContact(string botName, string userId)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);

            // Need confirm
            throw new System.NotImplementedException();
        }

        public IEnumerable<Contact> GetContacts(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            var result = restClientHelper.ExecuteGet<ContactListResult>($"bot/get_contacts/{botName}?tag={tag}");

            return result.Contacts;
        }

        public IEnumerable<string> GetTags(string botName)
        {
            ValidateInputParam(nameof(botName), botName);

            var result = restClientHelper.ExecuteGet<TagListResult>($"bot/list_tag/{botName}");

            return result.Tags;
        }

        public Result Push(string botName, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePost<Result>(
                $"bot/push/{botName}",
                new
                {
                    receiver = receivers,
                    payload
                });
        }

        public Result PushToTag(string botName, string tag, Payload payload)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePost<Result>(
                $"bot/push_to_tag/{botName}",
                new
                {
                    payload,
                    tag
                });
        }

        public Result SetWebhook(string botName, string url)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_webhook/{botName}",
                new
                {
                    url
                });
        }

        public Result SetWelcomeMessage(string botName, string message)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecutePost<Result>(
                  $"bot/set_welcome_msg/{botName}",
                  new
                  {
                      message
                  });
        }

        public Result UpdateTag(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            // Need confirm with onelab team
            throw new System.NotImplementedException();
        }

        public string GetContactPublicKey(string botName, string userId)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);

            var result = restClientHelper.ExecuteGet<PublicKeyResult>($"bot/get_contact_public_key/{botName}?contact={userId}");

            return result.PublicKey;
        }

        public Result SetContactKey(string botName, string userId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_key_to_contact/{botName}",
                new
                {
                    contact = userId,
                    encryption_key = hashKey,
                    hash_id = hashId
                });
        }

        public Result SetHashKey(string botName, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_hash_key/{botName}",
                new
                {
                    hash_key = hashKey,
                    hash_id = hashId
                });
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateInputParam(string paramName, IEnumerable<string> paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!paramValue.Any())
            {
                throw new ArgumentOutOfRangeException($"{paramName} does not contain any item");
            }
        }
    }
}