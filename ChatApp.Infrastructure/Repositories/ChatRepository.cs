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
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _db;

        public ChatRepository(ApplicationDbContext db) => _db = db;

        public async Task<Chat> GetOrCreateAsync(string u1, string u2)
        {
            var user1 = string.Compare(u1, u2) < 0 ? u1 : u2;
            var user2 = string.Compare(u1, u2) < 0 ? u2 : u1;

            var chat = await _db.Chats
                .FirstOrDefaultAsync(x =>
                    x.User1Id == user1 &&
                    x.User2Id == user2);

            if (chat != null)
                return chat;

            chat = new Chat
            {
                User1Id = user1,
                User2Id = user2
            };

            _db.Chats.Add(chat);
            return chat;
        }

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }

}
