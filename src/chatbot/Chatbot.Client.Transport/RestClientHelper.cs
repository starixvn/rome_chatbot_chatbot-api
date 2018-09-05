namespace Chatbot.Client.Transport
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Client.Models;
    using RestSharp;

    public class RestClient : Helpers.IRestClient
    {
        private readonly RestSharp.IRestClient restClient;

        public RestClient(RestSharp.IRestClient restClient = null)
        {
            this.restClient = restClient ?? CreateDefaultClient();
        }

        public Task<T> ExecuteGetAsync<T>(string endPoint, object data = null)
            => ExecuteSync<T>(endPoint, Method.GET, data);

        public Task<T> ExecutePostAsync<T>(string endPoint, object data = null)
            => ExecuteSync<T>(endPoint, Method.POST, data);

        public static RestSharp.IRestClient CreateDefaultClient()
           => new RestSharp.RestClient
           {
               BaseUrl = new Uri(ChatbotClientManager.BotClientConfigs.ApiBaseUrl),
               Authenticator = new HmacAuthenticator(ChatbotClientManager.BotClientConfigs.ApiKey)
           };

        private async Task<T> ExecuteSync<T>(string endPoint, Method method, object data)
        {
            ValidateEndPoint(endPoint);

            var request = new RestRequest(endPoint, method);

            if (data != null)
            {
                request.AddJsonBody(data);
            }

            var response = await restClient.ExecuteTaskAsync<T>(request).ConfigureAwait(false);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            if(response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                await Task.FromException(new ChatbotClientException(true, response.Content));
            }

            throw new InvalidOperationException(
                string.Join(
                    "\r\n",
                    $"Content: {response.Content}",
                    $"ErrorMessage: {response.ErrorMessage}"));
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }
    }
}