using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.ChatRequests.Commands.IncomingChatRequest
{
    public record IncomingChatRequestDto(
        long RequestId,
        string FromUserId,
        string FromUserName
    );

}
