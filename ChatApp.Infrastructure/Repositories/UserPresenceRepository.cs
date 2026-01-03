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
    public class UserPresenceRepository : IUserPresenceRepository
    {
        private readonly ApplicationDbContext _db;

        public UserPresenceRepository(ApplicationDbContext db)
            => _db = db;

        public async Task UpdateLastSeenAsync(string userId, DateTime lastSeen)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;

            user.LastSeenAt = lastSeen;
            await _db.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastSeenAsync(string userId)
        {
            return await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => u.LastSeenAt)
                .FirstOrDefaultAsync();
        }
    }

}
