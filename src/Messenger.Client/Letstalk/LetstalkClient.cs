namespace Messenger.Client.Letstalk
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

        public Result AddContactToTag(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePost<Result>(
                $"bot/modify_contact_tag/{botId}",
                new
                {
                    users,
                    tag
                });
        }

        public Result RemoveContactFromTag(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_contact_tag/{botId}",
                new
                {
                    contact = users,
                    tag
                });
        }

        public Result DeleteTag(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_tag/{botId}",
                new
                {
                    tag
                });
        }

        public Contact GetProfile(int botId, string userId)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);

            var result = restClientHelper.ExecuteGet<ProfileResult>($"bot/get_profile/{botId}/user/{userId}");

            return new Contact
            {
                UserId = userId,
                AvatarUrl = result.AvatarUrl,
                DisplayName = result.DisplayName
            };
        }

        public IEnumerable<Contact> GetContacts(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            var result = restClientHelper.ExecuteGet<ContactListResult>($"bot/get_contacts/{botId}/tag/{tag}");

            return result.Contacts;
        }

        public IEnumerable<string> GetTags(int botId)
        {
            ValidateInputParam(nameof(botId), botId);

            var result = restClientHelper.ExecuteGet<TagListResult>($"bot/list_tag/{botId}");

            return result.Tags;
        }

        public Result Push(int botId, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePost<Result>(
                $"bot/push/{botId}",
                new
                {
                    receiver = receivers,
                    payload
                });
        }

        public Result PushToTag(int botId, string tag, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePost<Result>(
                $"bot/push_to_tag/{botId}/tag/{tag}",
                new
                {
                    payload
                });
        }

        public Result SetWebhook(int botId, string url)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_webhook/{botId}",
                new
                {
                    url
                });
        }

        public Result SetWelcomeMessage(int botId, string message)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecutePost<Result>(
                  $"bot/set_welcome_msg/{botId}",
                  new
                  {
                      message
                  });
        }

        public Result AddTag(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePost<Result>(
                $"bot/modify_tag/{botId}",
                new
                {
                    tag
                });
        }

        public string GetUserPublicKey(int botId, string userId)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);

            var result = restClientHelper.ExecuteGet<PublicKeyResult>($"bot/get_user_public_key/{botId}/user/{userId}");

            return result.PublicKey;
        }

        public Result SetUserKey(int botId, string userId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_key_to_user/{botId}/user/{userId}",
                new
                {
                    encryption_key = hashKey,
                    hash_id = hashId
                });
        }

        public Result SetHashKey(int botId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_hash_key/{botId}",
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