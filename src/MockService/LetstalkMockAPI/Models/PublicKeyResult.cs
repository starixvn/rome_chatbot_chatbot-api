using Newtonsoft.Json;

namespace LetstalkMockAPI.Models
{
    public class PublicKeyResult : Result
    {
        [JsonProperty(PropertyName = "public_key")]
        public string PublicKey { get; set; }
    }
}
