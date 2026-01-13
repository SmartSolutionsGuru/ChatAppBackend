using System;

namespace ChatApp.Domain.Entities
{
    public enum CallStatus
    {
        Completed = 1,  // Call was answered and ended normally
        Missed = 2,     // Receiver didn't answer (timeout or offline)
        Rejected = 3,   // Receiver declined the call
        Busy = 4        // Receiver was already in another call
    }

    public enum CallType
    {
        Audio = 1,
        Video = 2
    }

    public class CallRecord
    {
        public long Id { get; set; }
        
        /// <summary>
        /// Unique identifier for the call session (used for signaling)
        /// </summary>
        public Guid CallId { get; set; }
        
        /// <summary>
        /// Associated chat between the two users
        /// </summary>
        public long ChatId { get; set; }
        public Chat Chat { get; set; } = default!;
        
        /// <summary>
        /// User who initiated the call
        /// </summary>
        public string CallerId { get; set; } = default!;
        
        /// <summary>
        /// User who received the call
        /// </summary>
        public string ReceiverId { get; set; } = default!;
        
        /// <summary>
        /// Audio or Video call
        /// </summary>
        public CallType Type { get; set; }
        
        /// <summary>
        /// Final outcome of the call
        /// </summary>
        public CallStatus Status { get; set; }
        
        /// <summary>
        /// When the call was initiated
        /// </summary>
        public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// When the call was answered (null if not answered)
        /// </summary>
        public DateTime? AnsweredAt { get; set; }
        
        /// <summary>
        /// When the call ended
        /// </summary>
        public DateTime EndedAt { get; set; }
        
        /// <summary>
        /// Duration in seconds (0 if not answered)
        /// </summary>
        public int DurationSeconds { get; set; } = 0;
    }
}
