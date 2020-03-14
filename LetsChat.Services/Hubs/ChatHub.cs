using LetsChat.Services.DataAccess;
using LetsChat.Services.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LetsChat.Services.Hubs
{
    public class ChatHub : Hub
    {
        private IMessageRepository _repo;
        public ChatHub(IMessageRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Joins the indicated chat room
        /// </summary>
        public Task JoinRoom(string room)
        {
            if (string.IsNullOrEmpty(room))
                return Task.CompletedTask;

            return Groups.AddToGroupAsync(Context.ConnectionId, room);
        }

        /// <summary>
        /// Leaves the indicated chat room
        /// </summary>
        public Task LeaveRoom(string room)
        {
            if (string.IsNullOrEmpty(room))
                return Task.CompletedTask;

            return Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        }

        public async Task PostMessage(ChatMessage msg)
        {
            await _repo.SaveMessage(msg);
        }
    }
}
