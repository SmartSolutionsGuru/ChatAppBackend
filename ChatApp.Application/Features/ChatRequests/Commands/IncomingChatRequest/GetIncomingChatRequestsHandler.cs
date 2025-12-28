using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.ChatRequests.Commands.IncomingChatRequest
{
    public class GetIncomingChatRequestsHandler
      : IRequestHandler<GetIncomingChatRequestsQuery, List<IncomingChatRequestDto>>
    {
        private readonly IChatRequestRepository _requests;
        private readonly ICurrentUserService _currentUser;

        public GetIncomingChatRequestsHandler(
            IChatRequestRepository requests,
            ICurrentUserService currentUser)
        {
            _requests = requests;
            _currentUser = currentUser;
        }

        public Task<List<IncomingChatRequestDto>> Handle(
            GetIncomingChatRequestsQuery request,
            CancellationToken ct)
        {
            return _requests.GetIncomingAsync(
                _currentUser.UserId,
                ct);
        }
    }
}
