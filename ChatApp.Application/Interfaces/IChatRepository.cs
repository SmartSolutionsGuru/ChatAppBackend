using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(long chatId);
        Task<Chat> GetOrCreateAsync(string user1, string user2);
        Task SaveChangesAsync();
    }

}
