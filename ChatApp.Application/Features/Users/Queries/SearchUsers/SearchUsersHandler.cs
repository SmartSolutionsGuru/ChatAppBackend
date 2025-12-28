using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Users.Queries.SearchUsers
{
    public class SearchUsersHandler
    : IRequestHandler<SearchUsersQuery, List<UserSearchItemDto>>
    {
        private readonly IUserSearchRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public SearchUsersHandler(
            IUserSearchRepository repo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public Task<List<UserSearchItemDto>> Handle(
            SearchUsersQuery request,
            CancellationToken ct)
        {
            return _repo.SearchAsync(
                _currentUser.UserId,
                request.Term);
        }
    }

}
