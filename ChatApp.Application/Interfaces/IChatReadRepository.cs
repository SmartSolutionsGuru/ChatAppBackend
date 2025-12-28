using ChatApp.Application.Features.Chat.Queries.GetMyChats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IChatReadRepository
    {
        Task<List<ChatListItemDto>> GetMyChatsAsync(string userId);
    }

}
