namespace Alpha.Messenger.Client.Models
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
}