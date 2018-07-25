namespace Chatbot.Client.Helpers
{
    using System.Threading.Tasks;

    public interface IRestClient
    {
        Task<T> ExecuteGetAsync<T>(string endPoint, object data = null);

        Task<T> ExecutePostAsync<T>(string endPoint, object data = null);
    }
}