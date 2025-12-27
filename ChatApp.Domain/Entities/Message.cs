using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public enum MessageStatus
    {
        Sent = 1,
        Delivered = 2,
        Read = 3
    }

    public class Message
    {
        public long Id { get; set; }

        public long ChatId { get; set; }
        public Chat Chat { get; set; } = default!;

        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;

        public string Content { get; set; } = default!;

        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
