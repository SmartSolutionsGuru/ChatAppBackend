using System;

namespace ChatApp.Application.Interfaces
{
    /// <summary>
    /// Tracks active calls in-memory (ephemeral state, not persisted)
    /// </summary>
    public interface IActiveCallTracker
    {
        /// <summary>
        /// Register a new pending call (caller initiated, waiting for answer)
        /// </summary>
        void RegisterPendingCall(Guid callId, long chatId, string callerId, string receiverId, string callType);

        /// <summary>
        /// Mark a call as connected (answered)
        /// </summary>
        void MarkCallConnected(Guid callId);

        /// <summary>
        /// End and remove a call from tracking
        /// </summary>
        void EndCall(Guid callId);

        /// <summary>
        /// Check if a user is currently in an active call
        /// </summary>
        bool IsUserInCall(string userId);

        /// <summary>
        /// Check if a user has a pending incoming call
        /// </summary>
        bool HasPendingCall(string userId);

        /// <summary>
        /// Get the other party in a call
        /// </summary>
        string? GetOtherParty(Guid callId, string userId);

        /// <summary>
        /// Get call info by CallId
        /// </summary>
        ActiveCallInfo? GetCallInfo(Guid callId);

        /// <summary>
        /// Get pending call for a receiver
        /// </summary>
        ActiveCallInfo? GetPendingCallForReceiver(string receiverId);
    }

    public class ActiveCallInfo
    {
        public Guid CallId { get; set; }
        public long ChatId { get; set; }
        public string CallerId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string CallType { get; set; } = default!;
        public DateTime InitiatedAt { get; set; }
        public DateTime? ConnectedAt { get; set; }
        public bool IsConnected { get; set; }
    }
}
