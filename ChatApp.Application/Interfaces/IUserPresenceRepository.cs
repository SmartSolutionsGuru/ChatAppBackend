using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IUserPresenceRepository
    {
        Task UpdateLastSeenAsync(string userId, DateTime lastSeen);
        Task<DateTime?> GetLastSeenAsync(string userId);
    }

}
