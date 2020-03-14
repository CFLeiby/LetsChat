using LetsChat.Services.Models;
using System.Threading.Tasks;

namespace LetsChat.Services.DataAccess
{
    /// <summary>
    /// Interface that defines expected functions required by various services 
    /// and service classes for getting/updating the persistence layer 
    /// </summary>
    public interface IMessageRepository
    {
        Task SaveMessage(ChatMessage message);
    }
}
