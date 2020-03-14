using LetsChat.Services.DataAccess;

namespace LetsChat.Services.Models
{
    /// <summary>
    /// Contains information about a chat room definition
    /// </summary>
    public class ChatRoom
    {
        public ChatRoom(IChatRoomData data)
        {
            Name = data.Name;
        }

        /// <summary>
        /// Name of the chat room
        /// </summary>
        public string Name { get; set; }        
    }
}
