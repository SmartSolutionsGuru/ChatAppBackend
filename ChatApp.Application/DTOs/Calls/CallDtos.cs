using System;

namespace ChatApp.Application.DTOs.Calls
{
    // ===================== Signaling DTOs (Ephemeral - Not Persisted) =====================
    
    /// <summary>
    /// Sent by caller to initiate a call
    /// </summary>
    public record InitiateCallRequest(
        long ChatId,
        string ReceiverId,
        string CallType,       // "audio" or "video"
        string Offer           // SDP offer from WebRTC
    );

    /// <summary>
    /// Sent to receiver when someone is calling them
    /// </summary>
    public record IncomingCallPayload(
        Guid CallId,
        long ChatId,
        string CallerId,
        string CallerName,
        string CallType,
        string Offer
    );

    /// <summary>
    /// Sent by receiver to accept the call
    /// </summary>
    public record AcceptCallRequest(
        Guid CallId,
        string Answer          // SDP answer from WebRTC
    );

    /// <summary>
    /// Sent to caller when call is accepted
    /// </summary>
    public record CallAcceptedPayload(
        Guid CallId,
        string Answer
    );

    /// <summary>
    /// Sent by receiver to reject the call
    /// </summary>
    public record RejectCallRequest(
        Guid CallId
    );

    /// <summary>
    /// Sent by caller to cancel the call (timeout or user cancelled)
    /// </summary>
    public record CancelCallRequest(
        Guid CallId
    );

    /// <summary>
    /// Sent by either party to end an active call
    /// </summary>
    public record EndCallRequest(
        Guid CallId
    );

    /// <summary>
    /// ICE candidate exchange for WebRTC
    /// </summary>
    public record IceCandidateRequest(
        Guid CallId,
        string Candidate       // JSON serialized ICE candidate
    );

    /// <summary>
    /// Notifies about call failure
    /// </summary>
    public record CallFailedPayload(
        Guid CallId,
        string Reason          // "user_offline", "user_busy", "timeout"
    );

    /// <summary>
    /// Notifies when call is rejected
    /// </summary>
    public record CallRejectedPayload(
        Guid CallId
    );

    /// <summary>
    /// Notifies when call is cancelled
    /// </summary>
    public record CallCancelledPayload(
        Guid CallId
    );

    /// <summary>
    /// Notifies when call ends
    /// </summary>
    public record CallEndedPayload(
        Guid CallId,
        int DurationSeconds
    );

    /// <summary>
    /// ICE candidate received from peer
    /// </summary>
    public record IceCandidatePayload(
        Guid CallId,
        string Candidate
    );

    // ===================== Persistence DTOs (Stored in Database) =====================

    /// <summary>
    /// Call history item for display
    /// </summary>
    public record CallHistoryItem(
        long Id,
        Guid CallId,
        long ChatId,
        string OtherUserId,
        string OtherUserName,
        string CallType,       // "audio" or "video"
        string Status,         // "completed", "missed", "rejected", "busy"
        bool IsOutgoing,       // true if current user initiated the call
        DateTime InitiatedAt,
        DateTime? AnsweredAt,
        DateTime EndedAt,
        int DurationSeconds
    );
}
