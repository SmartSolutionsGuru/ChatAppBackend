using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetChatMessages
{
    public class GetChatMessagesHandler
    : IRequestHandler<GetChatMessagesQuery, List<MessageDto>>
    {
        private readonly IMessageRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public GetChatMessagesHandler(
            IMessageRepository repo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public Task<List<MessageDto>> Handle(
            GetChatMessagesQuery request,
            CancellationToken ct)
        {
            return _repo.GetChatMessagesAsync(
                request.ChatId,
                _currentUser.UserId);
        }
    }

}
