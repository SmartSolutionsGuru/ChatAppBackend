using ChatApp.Application.Features.Chat.Queries.GetMyChats;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatReadRepository : IChatReadRepository
    {
        private readonly ApplicationDbContext _db;

        public ChatReadRepository(ApplicationDbContext db)
            => _db = db;

        public async Task<List<ChatListItemDto>> GetMyChatsAsync(string userId)
        {
            return await (
                from c in _db.Chats
                join u1 in _db.Users on c.User1Id equals u1.Id
                join u2 in _db.Users on c.User2Id equals u2.Id
                where c.User1Id == userId || c.User2Id == userId
                select new ChatListItemDto
                {
                    ChatId = c.Id,
                    OtherUserId = c.User1Id == userId ? c.User2Id : c.User1Id,
                    OtherUserName = c.User1Id == userId
                        ? u2.UserName!
                        : u1.UserName!,
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                    LastMessageAt = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.CreatedAt)
                        .FirstOrDefault(),
                    UnreadCount = c.Messages
                        .Count(m => m.ReceiverId == userId && m.Status != Domain.Entities.MessageStatus.Read)
                }
            )
            .OrderByDescending(x => x.LastMessageAt)
            .ToListAsync();
        }

    }

}
