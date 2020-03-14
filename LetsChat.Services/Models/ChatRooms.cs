using LetsChat.Services.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsChat.Services.Models
{
    /// <summary>
    /// Provides methods for working with sets of <see cref="ChatRoom"/> objects
    /// </summary>
    public class ChatRooms
    {
        private readonly IRepository _repo;

        public ChatRooms(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ChatRoom>> GetChatRooms()
        {
            var data = await _repo.GetRooms();
            if (data == null)
                return new List<ChatRoom>();

            return data.Select(r => new ChatRoom(r));
        }
    }
}
