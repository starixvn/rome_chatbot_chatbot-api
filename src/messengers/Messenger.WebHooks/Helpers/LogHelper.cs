namespace Messenger.WebHooks.Helpers
{
    using System.Collections.Specialized;
    using System.Text;
    using Messenger.WebHooks.Models;
    using Newtonsoft.Json;

    public static class LogHelper
    {
        public static string BuildRequestContent(NameValueCollection headers, HookEvent body)
        {
            var contentBuilder = new StringBuilder().AppendLine();

            contentBuilder.AppendLine("REQUEST HEADER");
            contentBuilder.AppendLine($" \tContent-Type: {headers["Content-Type"]}");
            contentBuilder.AppendLine($" \tAuthorization: {headers["Authorization"]}");

            contentBuilder.AppendLine("REQUEST BODY");
            contentBuilder.Append(JsonConvert.SerializeObject(body, Formatting.Indented));

            return contentBuilder.ToString();
        }
    }
}