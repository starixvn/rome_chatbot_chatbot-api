namespace Chatbot.Client.Transport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Client.Configurations;
    using Chatbot.Client.Models;
    using NSubstitute;
    using Xunit;

    public class RestClientHelperTests
    {
        private readonly RestSharp.IRestClient restsharpClient;
        private readonly RestClient restClient;

        public RestClientHelperTests()
        {
            restsharpClient = Substitute.For<RestSharp.IRestClient>();

            var config = new BotClientConfigs("http://dummy.com", "apiKey");
            restClient = new RestClient(restsharpClient);

            ChatbotClientManager
                .UseConfig(config)
                .UseRestClient(restClient);
        }

        [Fact]
        public void ExecuteGetAsync_Always_ResponseData()
        {
            var endPoint = "/endpoint";
            var data = "my_data";

            var restResponse = Substitute.For<RestSharp.IRestResponse<string>>();
            restResponse.IsSuccessful.Returns(true);
            restResponse.Data.Returns("Result");
            restsharpClient
                .ExecuteTaskAsync<string>(Arg.Any<RestSharp.IRestRequest>())
                .Returns(Task.FromResult(restResponse));

            var result = restClient
                .ExecuteGetAsync<string>(endPoint, data)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("Result", result);
        }

        [Fact]
        public void ExecuteGetAsync_ServiceIsUnvailable_ThrowChatbotClientException()
        {
            var endPoint = "/endpoint";
            var data = "my_data";

            var restResponse = Substitute.For<RestSharp.IRestResponse<string>>();
            restResponse.IsSuccessful.Returns(false);
            restResponse.StatusCode.Returns(System.Net.HttpStatusCode.ServiceUnavailable);
            restsharpClient
                .ExecuteTaskAsync<string>(Arg.Any<RestSharp.IRestRequest>())
                .Returns(Task.FromResult(restResponse));

            Assert.Throws<ChatbotClientException>(() =>
            restClient
                .ExecuteGetAsync<string>(endPoint, data)
                .GetAwaiter()
                .GetResult());
        }

        [Fact]
        public void ExecuteGetAsync_OtherError_ThrowInvalidOperationException()
        {
            var endPoint = "/endpoint";
            var data = "my_data";

            var restResponse = Substitute.For<RestSharp.IRestResponse<string>>();
            restResponse.IsSuccessful.Returns(false);
            restsharpClient
                .ExecuteTaskAsync<string>(Arg.Any<RestSharp.IRestRequest>())
                .Returns(Task.FromResult(restResponse));

            Assert.Throws<InvalidOperationException>(() =>
            restClient
                .ExecuteGetAsync<string>(endPoint, data)
                .GetAwaiter()
                .GetResult());
        }
    }
}