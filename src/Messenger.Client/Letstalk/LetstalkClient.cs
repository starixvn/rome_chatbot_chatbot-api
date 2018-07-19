namespace Messenger.Client.Letstalk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly int botId;
        private readonly IRestClientHelper restClientHelper;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly JsonSerializerSettings DefaultJsonSerializeSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public LetstalkClient(
            IRestClientHelper restClientHelper = null,
            ICryptographyHelper cryptographyHelper = null,
            IMessengerSettings messengerSettings = null)
        {
            this.restClientHelper = restClientHelper ?? new RestClientHelper(RestClientHelper.CreateDefaultClient(), new JwtHelper());
            this.cryptographyHelper = cryptographyHelper ?? new CryptographyHelper();
            this.messengerSettings = messengerSettings ?? AppSettings.Messenger;
            botId = this.messengerSettings.BotAccountId;
        }

        public Result AddContactToTag(string[] users, string tag)
            => AddContactToTagAsync(users, tag).Result;

        public Result RemoveContactFromTag(string[] users, string tag)
            => RemoveContactFromTagAsync(users, tag).Result;

        public Result DeleteTag(string tag)
            => DeleteTagAsync(tag).Result;

        public Contact GetProfile(string userId)
            => GetProfileAsync(userId).Result;

        public IEnumerable<Contact> GetContacts(string tag)
            => GetContactsAsync(tag).Result;

        public IEnumerable<string> GetTags(int botId)
            => GetTagsAsync(botId).Result;

        public Result Push(IEnumerable<string> receivers, string message)
            => PushAsync(receivers, message).Result;

        public Result Push(IEnumerable<string> receivers, Payload payload)
            => PushAsync(receivers, payload).Result;

        public Result PushToTag(string tag, Payload payload)
            => PushToTagAsync(tag, payload).Result;

        public Result SetWebhook(string url)
            => SetWebhookAsync(url).Result;

        public Result SetWelcomeMessage(string message)
            => SetWelcomeMessageAsync(message).Result;

        public Result AddTag(string tag)
            => AddTagAsync(tag).Result;

        public string GetUserPublicKey(string userId)
            => GetUserPublicKeyAsync(userId).Result;

        public Result SetUserKey(string userId, string hashId, string hashKey)
            => SetUserKeyAsync(userId, hashId, hashKey).Result;

        public Result SetHashKey(string hashId, string hashKey)
            => SetHashKeyAsync(hashId, hashKey).Result;

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

        public Result ChangeDisplayName(string botName)
        {
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