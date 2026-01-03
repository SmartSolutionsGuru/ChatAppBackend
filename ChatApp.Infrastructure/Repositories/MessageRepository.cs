using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public MessageRepository(ApplicationDbContext db)
            => _db = db;

        public async Task<List<MessageDto>> GetChatMessagesAsync(
            long chatId,
            string userId)
        {
            return await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    Status = m.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(
    long chatId,
    string readerId)
        {
            return await _db.Messages
                .Where(m =>
                    m.ChatId == chatId &&
                    m.ReceiverId == readerId &&
                    m.Status != MessageStatus.Read)
                .ToListAsync();
        }


        public Task<Message?> GetByIdAsync(long id)
        {
            return _db.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Message message)
       => await _db.Messages.AddAsync(message);

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }

}
