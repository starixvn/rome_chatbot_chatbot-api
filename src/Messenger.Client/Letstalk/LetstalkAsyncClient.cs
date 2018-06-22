namespace Messenger.Client.Letstalk
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Models;

    public partial class LetstalkClient
    {
        public Task<Result> AddContactToTagAsync(string botName, string userId, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });
        }

        public Task<Result> RemoveContactFromTagAsync(string botName, string userId, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncDelete<Result>(
                $"bot/modify_contact_tag/{botName}",
                new
                {
                    contact = userId,
                    tag
                });
        }

        public Task<Result> DeleteTagAsync(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            return restClientHelper.ExecuteAsyncDelete<Result>(
                $"bot/modify_tag/{botName}",
                new
                {
                    tag
                });
        }

        public Task<Contact> GetContactAsync(string botName, string userId)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);

            // Need confirm
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            var result = await restClientHelper.ExecuteAsyncGet<ContactListResult>($"bot/get_contacts/{botName}?tag={tag}");

            return result.Contacts;
        }

        public async Task<IEnumerable<string>> GetTagsAsync(string botName)
        {
            ValidateInputParam(nameof(botName), botName);

            var result = await restClientHelper.ExecuteAsyncGet<TagListResult>($"bot/list_tag/{botName}");

            return result.Tags;
        }

        public Task<Result> PushAsync(string botName, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/push/{botName}",
                new
                {
                    receiver = receivers,
                    payload
                });
        }

        public Task<Result> PushToTagAsync(string botName, string tag, Payload payload)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);
            ValidateInputParam(nameof(payload), payload);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/push_to_tag/{botName}",
                new
                {
                    payload,
                    tag
                });
        }

        public Task<Result> SetWebhookAsync(string botName, string url)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(url), url);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_webhook/{botName}",
                new
                {
                    url
                });
        }

        public Task<Result> SetWelcomeMessageAsync(string botName, string message)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(message), message);

            return restClientHelper.ExecuteAsyncPost<Result>(
                  $"bot/set_welcome_msg/{botName}",
                  new
                  {
                      message
                  });
        }

        public Task<Result> UpdateTagAsync(string botName, string tag)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(tag), tag);

            // Need confirm with onelab team
            throw new System.NotImplementedException();
        }

        public async Task<string> GetContactPublicKeyAsync(string botName, string userId)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);

            var result = await restClientHelper.ExecuteAsyncGet<PublicKeyResult>($"bot/get_contact_public_key/{botName}?contact={userId}");

            return result.PublicKey;
        }

        public Task<Result> SetContactKeyAsync(string botName, string userId, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_key_to_contact/{botName}",
                new
                {
                    contact = userId,
                    encryption_key = hashKey,
                    hash_id = hashId
                });
        }

        public Task<Result> SetHashKeyAsync(string botName, string hashId, string hashKey)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(hashId), hashId);
            ValidateInputParam(nameof(hashKey), hashKey);

            return restClientHelper.ExecuteAsyncPost<Result>(
                $"bot/set_hash_key/{botName}",
                new
                {
                    hash_key = hashKey,
                    hash_id = hashId
                });
        }
    }
}