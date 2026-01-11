using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetMyChats
{
    public class ChatListItemDto
    {
        public long ChatId { get; set; }
        public string OtherUserId { get; set; } = default!;
        public string OtherUserName { get; set; } = default!;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

}
