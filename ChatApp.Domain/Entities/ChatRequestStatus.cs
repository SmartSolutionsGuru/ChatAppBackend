using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public enum ChatRequestStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }

    public class ChatRequest
    {
        public long Id { get; set; }

        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;

        public ChatRequestStatus Status { get; set; } = ChatRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public void Accept() => Status = ChatRequestStatus.Accepted;
        public void Reject() => Status = ChatRequestStatus.Rejected;
    }
}
