namespace Messenger.Client.Letstalk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Messenger.Client.Models;
    using Newtonsoft.Json;

    public partial class LetstalkClient
    {
        public Task<Result> AddContactToTagAsync(string[] users, string tag)
        {
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/modify_contact_tag/{botId}",
                new Dictionary<string, object>
                {
                    { nameof(users), users },
                    { nameof(tag), tag }
                });
        }

        public Task<Result> RemoveContactFromTagAsync(string[] users, string tag)
        {
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDeleteAsync<Result>(
                $"bot/modify_contact_tag/{botId}",
                new Dictionary<string, object>
                {
                    { nameof(users), users },
                    { nameof(tag), tag }
                });
        }

        public Task<Result> DeleteTagAsync(string tag)
        {
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteDeleteAsync<Result>(
                $"bot/modify_tag/{botId}",
                new Dictionary<string, object>
                {
                    { nameof(tag), tag }
                });
        }

        public async Task<Contact> GetProfileAsync(string userId)
        {
            ValidateInputParam(nameof(userId), userId);

            var result = await restClientHelper.ExecuteGetAsync<ProfileResult>($"bot/get_profile/{botId}/user/{userId}");

            if (result.IsOk)
            {
                return new Contact
                {
                    UserId = userId,
                    AvatarUrl = result.AvatarUrl,
                    DisplayName = result.DisplayName
                };
            }

            return new Contact();
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(string tag)
        {
            ValidateInputParam(nameof(tag), tag);

            var result = await restClientHelper.ExecuteGetAsync<ContactListResult>($"bot/get_contacts/{botId}/tag/{tag}");

            return result.Contacts;
        }

        public async Task<IEnumerable<string>> GetTagsAsync(int botId)
        {
            var result = await restClientHelper.ExecuteGetAsync<TagListResult>($"bot/list_tag/{botId}");

            return result.Tags;
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, string message)
        {
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(message), message);

            var payload = new Payload();
            var formattedMessage = JsonConvert.SerializeObject(new { msg = message });

            payload.HashContentKey = cryptographyHelper.Hash(formattedMessage);
            payload.HashContent = cryptographyHelper.Encrypt(formattedMessage, messengerSettings.ChatKey, payload.HashContentKey);

            return PushAsync(receivers, payload);
        }

        public Task<Result> PushAsync(IEnumerable<string> receivers, Payload payload)
        {
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

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/push/{botId}",
                new Dictionary<string, object>
                {
                    { "receiver", receivers },
                    { "payload", JsonConvert.SerializeObject(payload, DefaultJsonSerializeSetting) }
                });
        }

        public Task<Result> PushToTagAsync(string tag, Payload payload)
        {
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/push_to_tag/{botId}/tag/{tag}",
                new Dictionary<string, object>
                {
                    { "payload", payload }
                });
        }

        public Task<Result> SetWebhookAsync(string url)
        {
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/set_webhook/{botId}",
                new Dictionary<string, object>
                {
                    { "url", url }
                });
        }

        public Task<Result> SetWelcomeMessageAsync(string message)
        {
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecutePostAsync<Result>(
                  $"bot/set_welcome_msg/{botId}",
                  new Dictionary<string, object>
                {
                    { "message", message }
                });
        }

        public Task<Result> AddTagAsync(string tag)
        {
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecutePostAsync<Result>(
               $"bot/modify_tag/{botId}",
               new Dictionary<string, object>
                {
                    { "tag", tag }
                });
        }

        public async Task<string> GetUserPublicKeyAsync(string userId)
        {
            ValidateInputParam(nameof(userId), userId);

            var result = await restClientHelper.ExecuteGetAsync<PublicKeyResult>($"bot/get_user_public_key/{botId}/user/{userId}");

            return result.PublicKey;
        }

        public Task<Result> SetUserKeyAsync(string userId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/set_key_to_user/{botId}/user/{userId}",
                new Dictionary<string, object>
                {
                    { "encryption_key", hashKey },
                    { "hash_id", hashId }
                });
        }

        public Task<Result> SetHashKeyAsync(string hashId, string hashKey)
        {
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecutePostAsync<Result>(
                $"bot/set_hash_key/{botId}",
                new Dictionary<string, object>
                {
                    { "hash_key", hashKey },
                    { "hash_id", hashId }
                });
        }

        public Task<Result> ChangeDisplayNameAsync(string botName)
        {
            ValidateInputParam(nameof(botName), botName);

            return restClientHelper.ExecutePutAsync<Result>(
                $"bot/change_display_name/{botId}",
                new Dictionary<string, object>
                {
                    { "display_name", botName }
                });
        }
    }
}