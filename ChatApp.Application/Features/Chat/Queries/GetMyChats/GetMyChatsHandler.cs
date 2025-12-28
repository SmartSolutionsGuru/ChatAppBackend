using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetMyChats
{
    public class GetMyChatsHandler
     : IRequestHandler<GetMyChatsQuery, List<ChatListItemDto>>
    {
        private readonly IChatReadRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public GetMyChatsHandler(
            IChatReadRepository repo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public Task<List<ChatListItemDto>> Handle(
            GetMyChatsQuery request,
            CancellationToken ct)
        {
            return _repo.GetMyChatsAsync(_currentUser.UserId);
        }
    }

}
