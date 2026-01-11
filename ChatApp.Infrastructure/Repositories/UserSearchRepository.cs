using ChatApp.Application.Features.Users.Queries.SearchUsers;
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
    public class UserSearchRepository : IUserSearchRepository
    {
        private readonly ApplicationDbContext _db;

        public UserSearchRepository(ApplicationDbContext db)
            => _db = db;

        public async Task<List<UserSearchItemDto>> SearchAsync(
            string currentUserId,
            string term)
        {
            // Users I already have a chat with
            var chatUserIds =
                _db.Chats
                   .Where(c => c.User1Id == currentUserId || c.User2Id == currentUserId)
                   .Select(c => c.User1Id == currentUserId ? c.User2Id : c.User1Id);

            // Users with pending requests (both directions)
            var pendingRequestUserIds =
                _db.ChatRequests
                   .Where(r =>
                       (r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                       && r.Status == ChatRequestStatus.Pending)
                   .Select(r => r.FromUserId == currentUserId ? r.ToUserId : r.FromUserId);

            return await _db.Users
                .Where(u =>
                    u.Id != currentUserId &&
                    u.UserName!.Contains(term) &&
                    !chatUserIds.Contains(u.Id) &&
                    !pendingRequestUserIds.Contains(u.Id))
                .Select(u => new UserSearchItemDto
                {
                    UserId = u.Id,
                    UserName = u.UserName!
                })
                .Take(20)
                .ToListAsync();
        }

        public async Task<string?> GetUserNameByIdAsync(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user?.UserName;
        }
    }

}
