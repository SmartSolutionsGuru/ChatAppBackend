using ChatApp.Application.Features.ChatRequests.Commands.IncomingChatRequest;
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

        public async Task<List<IncomingChatRequestDto>> GetIncomingAsync(
     string toUserId,
     CancellationToken ct)
        {
            return await (
                from r in _db.ChatRequests
                join u in _db.Users
                    on r.FromUserId equals u.Id
                where
                    r.ToUserId == toUserId &&
                    r.Status == ChatRequestStatus.Pending
                orderby r.CreatedAt descending
                select new IncomingChatRequestDto(
                    r.Id,
                    r.FromUserId,
                    u.UserName!
                )
            ).ToListAsync(ct);
        }
    }

}
