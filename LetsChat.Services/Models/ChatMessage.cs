namespace LetsChat.Services.Models
{
    /// <summary>
    /// Holds details of a chat message (both for transport to/from the client,
    /// as well as for storage in a message on our service bus)
    /// </summary>
    public class ChatMessage
    {
        public string ChatRoom { get; set; }
        public string MessageContent { get; set; }
        public string UserName { get; set; }
    }
}
