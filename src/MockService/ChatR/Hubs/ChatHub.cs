namespace ChatR.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using ChatR.Helpers;
    using ChatR.Models;
    using ChatR.Utils;
    using ChatR.ViewModels;
    using Microsoft.AspNet.SignalR;

    public class ChatHub : Hub
    {
        private InMemoryRepository _repository;
        private IRestClientHelper _restClientHelper;

        public ChatHub()
        {
            _repository = InMemoryRepository.GetInstance();
            _restClientHelper = new RestClientHelper();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userId = _repository.GetUserByConnectionId(Context.ConnectionId);
            if (userId != null)
            {
                ChatUser user = _repository.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    _repository.Remove(user);
                    return Clients.All.leaves(user.Id, user.Username, DateTime.Now);
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public void Send(ChatMessage message)
        {
            if (!string.IsNullOrEmpty(message.Content))
            {
                message.Content = HttpUtility.HtmlEncode(message.Content);
                HashSet<string> extractedURLs;
                message.Content = TextParser.TransformAndExtractUrls(message.Content, out extractedURLs);
                message.Timestamp = DateTime.Now;

                Clients.All.onMessageReceived(message);

                Push("apha.bot", new string[] { message.Username }, new Payload
                {
                    Id = 1,
                    From = 0,
                    UserId = "alpha.bot",
                    HashContent = "content"
                });
            }
        }

        public void Receive(PushMessageViewModel message)
        {
            if (message.Payload != null)
            {
                var payload = message.Payload;
                var chatMessage = new ChatMessage { Username = payload.UserId, Content = payload.HashContent };

                chatMessage.Content = HttpUtility.HtmlEncode(chatMessage.Content);
                HashSet<string> extractedURLs;
                chatMessage.Content = TextParser.TransformAndExtractUrls(chatMessage.Content, out extractedURLs);
                chatMessage.Timestamp = DateTime.Now;

                Clients.All.onMessageReceived(chatMessage);
            }
        }

        public void Joined()
        {
            ChatUser user = new ChatUser()
            {
                Id = Guid.NewGuid().ToString(),
                Username = Clients.Caller.username
            };
            _repository.Add(user);
            _repository.AddMapping(Context.ConnectionId, user.Id);
            Clients.All.joins(user.Id, Clients.Caller.username, DateTime.Now);
        }

        public ICollection<ChatUser> GetConnectedUsers()
        {
            return _repository.Users.ToList<ChatUser>();
        }

        public Result Push(string botName, IEnumerable<string> receivers, Payload payload)
        {
            ValidateInputParam(nameof(botName), botName);
            ValidateInputParam(nameof(receivers), receivers);
            ValidateInputParam(nameof(payload), payload);

            return _restClientHelper.Execute<Result>(
                $"bot/push/{botName}",
                RestSharp.Method.POST,
                new
                {
                    receiver = receivers,
                    payload
                });
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}