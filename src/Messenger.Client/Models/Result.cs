namespace Messenger.Client.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Result
    {
        public int Status { get; set; }

        public bool IsOk => Status == 1;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }
    }

    /// <summary>
    /// TODO: workaround to deserialize response
    /// </summary>
    internal class TagListResult : Result
    {
        public List<string> Tags { get; set; }
    }

    /// <summary>
    /// TODO: workaround to deserialize response
    /// </summary>
    internal class ContactListResult : Result
    {
        public List<Contact> Contacts { get; set; }
    }

    /// <summary>
    /// TODO: workaround to deserialize response
    /// </summary>
    internal class PublicKeyResult : Result
    {
        [JsonProperty(PropertyName = "public_key")]
        public string PublicKey { get; set; }
    }

    internal class ProfileResult : Result
    {
        private const int AccountDoesNotExistStatus = 21;

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        public bool IsAccountNotExistent => Status == AccountDoesNotExistStatus;
    }
}