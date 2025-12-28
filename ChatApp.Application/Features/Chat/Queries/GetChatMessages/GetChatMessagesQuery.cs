using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetChatMessages
{
    public record GetChatMessagesQuery(long ChatId)
    : IRequest<List<MessageDto>>;
}
