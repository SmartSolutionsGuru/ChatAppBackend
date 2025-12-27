using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRequestRepository
    {
        Task<bool> ExistsPendingAsync(string fromUserId, string toUserId);
        Task AddAsync(ChatRequest request);
        Task<ChatRequest?> GetByIdAsync(long id);
        Task SaveChangesAsync();
    }
}
