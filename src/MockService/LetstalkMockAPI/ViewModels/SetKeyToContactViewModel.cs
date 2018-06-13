using Newtonsoft.Json;

namespace LetstalkMockAPI.ViewModels
{
    public class SetKeyToContactViewModel
    {
        public string Contact { get; set; }

        [JsonProperty(PropertyName = "encryption_key")]
        public string EncryptionKey { get; set; }

        [JsonProperty(PropertyName = "hash_id")]
        public long HashId { get; set; }
    }
}
