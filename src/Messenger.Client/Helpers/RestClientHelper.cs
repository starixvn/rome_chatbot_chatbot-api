namespace Messenger.Client.Helpers
{
    using System;
    using System.Threading.Tasks;
    using RestSharp;

    public interface IRestClientHelper
    {
        T ExecuteGet<T>(string endPoint, object data = null) where T : new();

        T ExecutePost<T>(string endPoint, object data) where T : new();

        T ExecuteDelete<T>(string endPoint, object data) where T : new();

        Task<T> ExecuteAsyncGet<T>(string endPoint, object data = null) where T : new();

        Task<T> ExecuteAsyncPost<T>(string endPoint, object data) where T : new();

        Task<T> ExecuteAsyncDelete<T>(string endPoint, object data) where T : new();
    }

    public class RestClientHelper : IRestClientHelper
    {
        private readonly IRestClient restClient;

        public RestClientHelper(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        public T ExecuteGet<T>(string endPoint, object data = null)
            where T : new()
            => Execute<T>(endPoint, Method.GET, data);

        public T ExecutePost<T>(string endPoint, object data)
            where T : new()
            => Execute<T>(endPoint, Method.POST, data);

        public T ExecuteDelete<T>(string endPoint, object data)
            where T : new()
            => Execute<T>(endPoint, Method.DELETE, data);

        private T Execute<T>(string endPoint, Method method, object data)
            where T : new()
        {
            ValidateEndPoint(endPoint);

            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(endPoint, method);
            request.AddHeader("Content-Type", "application/json");

            if (data != null)
            {
                request.AddJsonBody(data);
            }

            var response = restClient.Execute<T>(request);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            throw new InvalidOperationException(response.ErrorMessage);
        }

        private async Task<T> ExecuteSync<T>(string endPoint, Method method, object data)
            where T : new()
        {
            ValidateEndPoint(endPoint);

            var request = new RestSharp.Serializers.Newtonsoft.Json.RestRequest(endPoint, method);
            request.AddHeader("Content-Type", "application/json");

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

        public Task<T> ExecuteAsyncGet<T>(string endPoint, object data = null) where T : new()
            => ExecuteSync<T>(endPoint, Method.GET, data);

        public Task<T> ExecuteAsyncPost<T>(string endPoint, object data) where T : new()
            => ExecuteSync<T>(endPoint, Method.POST, data);

        public Task<T> ExecuteAsyncDelete<T>(string endPoint, object data) where T : new()
            => ExecuteSync<T>(endPoint, Method.DELETE, data);
    }
}