using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<MessageDto>> GetChatMessagesAsync(
            long chatId,
            string userId);
    }
}
