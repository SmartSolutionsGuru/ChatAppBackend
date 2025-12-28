using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Users.Queries.SearchUsers
{
    public record SearchUsersQuery(string Term)
     : IRequest<List<UserSearchItemDto>>;
}
