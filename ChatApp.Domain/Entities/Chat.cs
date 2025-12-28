using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public class Chat
    {
        public long Id { get; set; }
        public string User1Id { get; set; } = default!;
        public string User2Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
