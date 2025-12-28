using ChatApp.Application.Features.Users.Queries.SearchUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IUserSearchRepository
    {
        Task<List<UserSearchItemDto>> SearchAsync(
            string currentUserId,
            string term);
    }
}
