using Newtonsoft.Json;

namespace LetstalkMockAPI.ViewModels
{
    public class ChangeDisplayNameViewModel
    {
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }
    }
}
