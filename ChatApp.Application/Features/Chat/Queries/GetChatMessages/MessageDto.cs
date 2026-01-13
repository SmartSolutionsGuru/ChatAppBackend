using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.GetChatMessages
{
    public class MessageDto
    {
        public long Id { get; set; }
        public string SenderId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }

        // Voice note properties
        public bool IsVoiceNote { get; set; }
        public string? VoiceNoteUrl { get; set; }
        public double? VoiceNoteDuration { get; set; }
        public double[]? VoiceNoteWaveform { get; set; }

        // Call message properties (WhatsApp-style)
        public bool IsCallMessage { get; set; }
        public string? CallType { get; set; }       // "audio" or "video"
        public int? CallDuration { get; set; }      // Duration in seconds
        public string? CallStatus { get; set; }     // "completed", "missed", "rejected"
    }

}
