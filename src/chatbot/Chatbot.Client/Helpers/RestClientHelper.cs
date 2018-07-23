namespace Chatbot.Client.Helpers
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Client.Configurations;
    using RestSharp;

    public interface IRestClientHelper
    {
        Task<T> ExecuteGetAsync<T>(string endPoint, object data = null);

        Task<T> ExecutePostAsync<T>(string endPoint, object data = null);
    }

    public class RestClientHelper : IRestClientHelper
    {
        private readonly IRestClient restClient;

        public RestClientHelper(IRestClient restClient = null)
        {
            this.restClient = restClient ?? CreateDefaultClient();
        }

        public Task<T> ExecuteGetAsync<T>(string endPoint, object data = null)
            => ExecuteSync<T>(endPoint, Method.GET, data);

        public Task<T> ExecutePostAsync<T>(string endPoint, object data = null)
            => ExecuteSync<T>(endPoint, Method.POST, data);

        public static IRestClient CreateDefaultClient()
           => new RestClient
           {
               BaseUrl = new Uri(AppSettings.BotClientConfigs.ApiBaseUrl),
               Authenticator = new HmacAuthenticator()
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

            throw new InvalidOperationException(response.ErrorMessage);
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