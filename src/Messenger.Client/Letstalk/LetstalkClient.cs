namespace Messenger.Client.Letstalk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public partial class LetstalkClient : IMessengerClient
    {
        private const int AESEncryptionMode = 5;
        private const int BusinessAccountType = 2;
        private const int ChatKeyVersion = 3;
        private readonly IRestClientHelper restClientHelper;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly JsonSerializerSettings DefaultJsonSerializeSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public LetstalkClient(
            IRestClientHelper restClientHelper,
            ICryptographyHelper cryptographyHelper,
            IMessengerSettings messengerSettings = null)
        {
            this.restClientHelper = restClientHelper;
            this.cryptographyHelper = cryptographyHelper;
            this.messengerSettings = messengerSettings ?? AppSettings.Messenger;
        }

        public Result AddContactToTag(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePost<Result>(
                $"bot/modify_contact_tag/{botId}",
                new Dictionary<string, object>
                {
                    { nameof(users), users },
                    { nameof(tag), tag }
                });
        }

        public Result RemoveContactFromTag(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_contact_tag/{botId}",
                new Dictionary<string, object>
                {
                    { "contact", users },
                    { nameof(tag), tag }
                });
        }

        public Result DeleteTag(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDelete<Result>(
                $"bot/modify_tag/{botId}",
                new Dictionary<string, object>
                {
                    { nameof(tag), tag }
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

        public Result Push(int botId, IEnumerable<string> receivers, string message)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(message), message);

            var payload = new Payload();
            var formattedMessage = JsonConvert.SerializeObject(new { msg = message });

            payload.HashContentKey = cryptographyHelper.Hash(formattedMessage);
            payload.HashContent = cryptographyHelper.Encrypt(formattedMessage, messengerSettings.ChatKey, payload.HashContentKey);

            return Push(botId, receivers, payload);
        }

        public Result Push(int botId, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            payload.UserId = messengerSettings.BotUserId;
            payload.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            payload.HashType = AESEncryptionMode;
            payload.From = BusinessAccountType;
            payload.Type = MessageType.Text;
            payload.Id = 100;
            payload.Receiver = receivers.ElementAt(0);
            payload.HashId = ChatKeyVersion;

            return restClientHelper.ExecutePost<Result>(
                $"bot/push/{botId}",
                new Dictionary<string, object>
                {
                    { "receiver", receivers },
                    { "payload", JsonConvert.SerializeObject(payload, DefaultJsonSerializeSetting) }
                });
        }

        public Result PushToTag(int botId, string tag, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePost<Result>(
                $"bot/push_to_tag/{botId}/tag/{tag}",
                new Dictionary<string, object>
                {
                    { "payload", payload }
                });
        }

        public Result SetWebhook(int botId, string url)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_webhook/{botId}",
                new Dictionary<string, object>
                {
                    { "url", url }
                });
        }

        public Result SetWelcomeMessage(int botId, string message)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecutePost<Result>(
                  $"bot/set_welcome_msg/{botId}",
                  new Dictionary<string, object>
                {
                    { "message", message }
                });
        }

        public Result AddTag(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePost<Result>(
                $"bot/modify_tag/{botId}",
                new Dictionary<string, object>
                {
                    { "tag", tag }
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
                new Dictionary<string, object>
                {
                    { "encryption_key", hashKey },
                    { "hash_id", hashId }
                });
        }

        public Result SetHashKey(int botId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePost<Result>(
                $"bot/set_hash_key/{botId}",
                new Dictionary<string, object>
                {
                    { "hash_key", hashKey },
                    { "hash_id", hashId }
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

        public Result ChangeDisplayName(int botId, string botName)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(botName), botName);

            return restClientHelper.ExecutePut<Result>(
                $"bot/change_display_name/{botId}",
                new Dictionary<string, object>
                {
                    { "display_name", botName }
                });
        }
    }
}