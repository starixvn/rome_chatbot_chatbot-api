namespace Messenger.Client.Letstalk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial class LetstalkClient
    {
        public Task<Result> AddContactToTagAsync(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/modify_contact_tag/{botId}",
                new
                {
                    users,
                    tag
                });
        }

        public Task<Result> RemoveContactFromTagAsync(int botId, string[] users, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(users), users);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncDelete<Result>(
                $"bot/modify_contact_tag/{botId}",
                new
                {
                    users,
                    tag
                });
        }

        public Task<Result> DeleteTagAsync(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncDelete<Result>(
                $"bot/modify_tag/{botId}",
                new
                {
                    tag
                });
        }

        public async Task<Contact> GetProfileAsync(int botId, string userId)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);

            var result = await restClientHelper.ExecuteAsyncGet<ProfileResult>($"bot/get_profile/{botId}/user/{userId}");

            return new Contact
            {
                UserId = userId,
                AvatarUrl = result.AvatarUrl,
                DisplayName = result.DisplayName
            };
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            var result = await restClientHelper.ExecuteAsyncGet<ContactListResult>($"bot/get_contacts/{botId}/tag/{tag}");

            return result.Contacts;
        }

        public async Task<IEnumerable<string>> GetTagsAsync(int botId)
        {
            ValidateInputParam(nameof(botId), botId);

            var result = await restClientHelper.ExecuteAsyncGet<TagListResult>($"bot/list_tag/{botId}");

            return result.Tags;
        }

        public Task<Result> PushAsync(int botId, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/push/{botId}",
                new
                {
                    receiver = receivers,
                    payload
                });
        }

        public Task<Result> PushToTagAsync(int botId, string tag, Payload payload)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/push_to_tag/{botId}/tag/{tag}",
                new
                {
                    payload
                });
        }

        public Task<Result> SetWebhookAsync(int botId, string url)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_webhook/{botId}",
                new
                {
                    url
                });
        }

        public Task<Result> SetWelcomeMessageAsync(int botId, string message)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecuteAsyncPost<Result>(
                  $"bot/set_welcome_msg/{botId}",
                  new
                  {
                      message
                  });
        }

        public Task<Result> AddTagAsync(int botId, string tag)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncPost<Result>(
               $"bot/modify_tag/{botId}",
               new
               {
                   tag
               });
        }

        public async Task<string> GetUserPublicKeyAsync(int botId, string userId)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);

            var result = await restClientHelper.ExecuteAsyncGet<PublicKeyResult>($"bot/get_user_public_key/{botId}/user/{userId}");

            return result.PublicKey;
        }

        public Task<Result> SetUserKeyAsync(int botId, string userId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_key_to_user/{botId}/user/{userId}",
                new
                {
                    encryption_key = hashKey,
                    hash_id = hashId
                });
        }

        public Task<Result> SetHashKeyAsync(int botId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botId), botId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_hash_key/{botId}",
                new
                {
                    hash_key = hashKey,
                    hash_id = hashId
                });
        }
    }
}