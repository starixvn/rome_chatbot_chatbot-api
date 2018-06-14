using ChatR.Models;
using ChatR.Utils;
using ChatR.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChatR.Hubs
{
    public class ChatHub : Hub
    {
        private InMemoryRepository _repository;

        public ChatHub()
        {
            _repository = InMemoryRepository.GetInstance();
        }

        #region IDisconnect and IConnected event handlers implementation

        /// <summary>
        /// Fired when a client disconnects from the system. The user associated with the client ID gets deleted from the list of currently connected users.
        /// </summary>
        /// <returns></returns>
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

        #endregion IDisconnect and IConnected event handlers implementation

        #region Chat event handlers

        /// <summary>
        /// Fired when a client pushes a message to the server.
        /// </summary>
        /// <param name="message"></param>
        public void Send(PushMessageViewModel message)
        {
            if (message.Payload != null)
            {
                var payload = message.Payload;
                var chatMessage = new ChatMessage { Username = payload.UserId, Content = payload.HashContent };

                // Sanitize input
                chatMessage.Content = HttpUtility.HtmlEncode(chatMessage.Content);
                // Process URLs: Extract any URL and process rich content (e.g. Youtube links)
                HashSet<string> extractedURLs;
                chatMessage.Content = TextParser.TransformAndExtractUrls(chatMessage.Content, out extractedURLs);
                chatMessage.Timestamp = DateTime.Now;
                Clients.All.onMessageReceived(chatMessage);
            }
        }

        /// <summary>
        /// Fired when a client joins the chat. Here round trip state is available and we can register the user in the list
        /// </summary>
        public void Joined()
        {
            ChatUser user = new ChatUser()
            {
                //Id = Context.ConnectionId,
                Id = Guid.NewGuid().ToString(),
                Username = Clients.Caller.username
            };
            _repository.Add(user);
            _repository.AddMapping(Context.ConnectionId, user.Id);
            Clients.All.joins(user.Id, Clients.Caller.username, DateTime.Now);
        }

        /// <summary>
        /// Invoked when a client connects. Retrieves the list of all currently connected users
        /// </summary>
        /// <returns></returns>
        public ICollection<ChatUser> GetConnectedUsers()
        {
            return _repository.Users.ToList<ChatUser>();
        }

        #endregion Chat event handlers
    }
}