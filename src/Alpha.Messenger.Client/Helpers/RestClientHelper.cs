namespace Alpha.Messenger.Client.Helpers
{
    using System;
    using Alpha.Messenger.Client.Configurations;
    using RestSharp;
    using RestSharp.Authenticators;

    /// <summary>
    /// This class simple first, will support async in next time
    /// </summary>
    public interface IRestClientHelper
    {
        T ExecuteGet<T>(string endPoint, object data = null) where T : new();

        T ExecutePost<T>(string endPoint, object data) where T : new();

        T ExecuteDelete<T>(string endPoint, object data) where T : new();
    }

    public class RestClientHelper : IRestClientHelper
    {
        private readonly IRestClient restClient;

        public RestClientHelper(IRestClient restClient = null)
        {
            this.restClient = restClient ?? CreateDefaultRestClient();
        }

        private static IRestClient CreateDefaultRestClient()
            => new RestClient(AppSettings.MessengerApiEndpoint)
            {
                Authenticator = new JwtAuthenticator("dummy_access_token")
            };

        public T ExecuteGet<T>(string endPoint, object data = null)
            where T : new()
            => ExecuteRequest<T>(endPoint, Method.GET, data);

        public T ExecutePost<T>(string endPoint, object data)
            where T : new()
            => ExecuteRequest<T>(endPoint, Method.POST, data);

        public T ExecuteDelete<T>(string endPoint, object data)
            where T : new()
            => ExecuteRequest<T>(endPoint, Method.DELETE, data);

        private T ExecuteRequest<T>(string endPoint, Method method, object data)
            where T : new()
        {
            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(endPoint, method);

            if (data != null)
            {
                request.AddObject(data);
            }

            var response = restClient.Execute<T>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            throw new InvalidOperationException(response.ErrorMessage);
        }
    }
}