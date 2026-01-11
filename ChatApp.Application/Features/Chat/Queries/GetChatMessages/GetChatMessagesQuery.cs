using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetChatMessages
{
    public record GetChatMessagesQuery(
        long ChatId,
        int PageSize = 30,
        long? BeforeMessageId = null  // For pagination: get messages before this ID
    ) : IRequest<PaginatedMessagesDto>;

    public class PaginatedMessagesDto
    {
        public List<MessageDto> Messages { get; set; } = new();
        public bool HasMore { get; set; }
        public long? OldestMessageId { get; set; }
    }
}
