using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
namespace ChatApp.Application.Features.Chat.Queries.GetMyChats
{


    public record GetMyChatsQuery : IRequest<List<ChatListItemDto>>;

}
