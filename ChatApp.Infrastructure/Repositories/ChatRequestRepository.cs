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
    public class ChatRequestRepository : IChatRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ChatRequestRepository(ApplicationDbContext db) => _db = db;

        public Task<bool> ExistsPendingAsync(string from, string to)
            => _db.ChatRequests.AnyAsync(x =>
                x.FromUserId == from &&
                x.ToUserId == to &&
                x.Status == ChatRequestStatus.Pending);

        public async Task AddAsync(ChatRequest request)
            => await _db.ChatRequests.AddAsync(request);

        public Task<ChatRequest?> GetByIdAsync(long id)
            => _db.ChatRequests.FindAsync(id).AsTask();

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }

}
