using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<PaginatedMessagesDto> GetChatMessagesAsync(
            long chatId,
            string userId,
            int pageSize = 30,
            long? beforeMessageId = null);

        Task<List<MessageDto>> SearchMessagesAsync(
            long chatId,
            string searchTerm,
            int maxResults = 50);

        Task<Message?> GetByIdAsync(long id);
        Task AddAsync(Message message);
        Task SaveChangesAsync();
        Task<List<Message>> GetUnreadMessagesAsync(long chatId, string readerId);
        Task<List<Message>> GetUndeliveredMessagesForUserAsync(string recipientId);
    }
}
