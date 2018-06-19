namespace Messenger.WebHooks.Models
{
    using Newtonsoft.Json;

    public class HookEvent
    {
        public string Type { get; set; }

        public long TimeStamp { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public string EncryptedPayload { get; set; }

        public EventSource Source { get; set; }
    }
}