using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Users.Queries.SearchUsers
{
    public class UserSearchItemDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
    }

}
