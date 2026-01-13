using ChatApp.Application.DTOs.Calls;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IUserConnectionTracker _tracker;
    private readonly IUserPresenceRepository _presenceRepo;
    private readonly IMessageRepository _messageRepo;
    private readonly IMessageNotifier _messageNotifier;
    private readonly IActiveCallTracker _callTracker;
    private readonly ICallRepository _callRepo;

    public ChatHub(
        IUserConnectionTracker tracker,
        IUserPresenceRepository presenceRepo,
        IMessageRepository messageRepo,
        IMessageNotifier messageNotifier,
        IActiveCallTracker callTracker,
        ICallRepository callRepo)
    {
        _tracker = tracker;
        _presenceRepo = presenceRepo;
        _messageRepo = messageRepo;
        _messageNotifier = messageNotifier;
        _callTracker = callTracker;
        _callRepo = callRepo;
    }
    public async Task Typing(long chatId)
    {
        var userId = Context.UserIdentifier;

        if (userId == null) return;

        await Clients
            .OthersInGroup(chatId.ToString())
            .SendAsync("typing", new
            {
                chatId,
                userId
            });
    }

    public async Task StopTyping(long chatId)
    {
        var userId = Context.UserIdentifier;

        if (userId == null) return;

        await Clients
            .OthersInGroup(chatId.ToString())
            .SendAsync("stopTyping", new
            {
                chatId,
                userId
            });
    }

    public async Task RecordingVoice(long chatId)
    {
        var userId = Context.UserIdentifier;

        if (userId == null) return;

        await Clients
            .OthersInGroup(chatId.ToString())
            .SendAsync("recordingVoice", new
            {
                chatId,
                userId
            });
    }

    public async Task StopRecordingVoice(long chatId)
    {
        var userId = Context.UserIdentifier;

        if (userId == null) return;

        await Clients
            .OthersInGroup(chatId.ToString())
            .SendAsync("stopRecordingVoice", new
            {
                chatId,
                userId
            });
    }

    public async Task JoinChat(long chatId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            chatId.ToString()
        );
    }

    public async Task LeaveChat(long chatId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            chatId.ToString()
        );
    }

    // ================= PRESENCE =================

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (userId != null)
        {
            var isFirstConnection = _tracker.UserConnected(userId);

            if (isFirstConnection)
            {
                await Clients.All.SendAsync("userOnline", userId);

                // Deliver all pending messages to this user
                await DeliverPendingMessagesAsync(userId);
            }
        }

        await base.OnConnectedAsync(); // ✅ ALWAYS EXECUTE
    }

    private async Task DeliverPendingMessagesAsync(string recipientId)
    {
        var pendingMessages = await _messageRepo.GetUndeliveredMessagesForUserAsync(recipientId);

        foreach (var msg in pendingMessages)
        {
            msg.Status = MessageStatus.Delivered;
            await _messageNotifier.NotifyMessageDelivered(msg.ChatId, msg.Id, msg.SenderId);
        }

        if (pendingMessages.Count > 0)
        {
            await _messageRepo.SaveChangesAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (userId != null)
        {
            var isLastConnection = _tracker.UserDisconnected(userId);

            if (isLastConnection)
            {
                var lastSeen = DateTime.UtcNow;

                await _presenceRepo.UpdateLastSeenAsync(userId, lastSeen);

                await Clients.All.SendAsync("userOffline", new
                {
                    userId,
                    lastSeen
                });
            }
        }

        await base.OnDisconnectedAsync(exception); // ✅ ALWAYS
    }

    // ================= CALL SIGNALING =================

    /// <summary>
    /// Caller initiates a call to another user
    /// </summary>
    public async Task InitiateCall(InitiateCallRequest request)
    {
        var callerId = Context.UserIdentifier;
        if (callerId == null) return;

        var callId = Guid.NewGuid();

        // Check if receiver is online
        if (!_tracker.IsOnline(request.ReceiverId))
        {
            // Receiver offline → Immediate MISSED
            await Clients.Caller.SendAsync("callFailed", new CallFailedPayload(callId, "user_offline"));

            // Persist missed call
            await _callRepo.SaveCallRecordAsync(new CallRecord
            {
                CallId = callId,
                ChatId = request.ChatId,
                CallerId = callerId,
                ReceiverId = request.ReceiverId,
                Type = request.CallType == "video" ? CallType.Video : CallType.Audio,
                Status = CallStatus.Missed,
                InitiatedAt = DateTime.UtcNow,
                EndedAt = DateTime.UtcNow
            });
            return;
        }

        // Check if receiver is already in a call
        if (_callTracker.IsUserInCall(request.ReceiverId))
        {
            await Clients.Caller.SendAsync("callFailed", new CallFailedPayload(callId, "user_busy"));
            
            // Persist busy call
            await _callRepo.SaveCallRecordAsync(new CallRecord
            {
                CallId = callId,
                ChatId = request.ChatId,
                CallerId = callerId,
                ReceiverId = request.ReceiverId,
                Type = request.CallType == "video" ? CallType.Video : CallType.Audio,
                Status = CallStatus.Busy,
                InitiatedAt = DateTime.UtcNow,
                EndedAt = DateTime.UtcNow
            });
            return;
        }

        // Check if caller is already in a call
        if (_callTracker.IsUserInCall(callerId))
        {
            await Clients.Caller.SendAsync("callFailed", new CallFailedPayload(callId, "already_in_call"));
            return;
        }

        // Register pending call
        _callTracker.RegisterPendingCall(callId, request.ChatId, callerId, request.ReceiverId, request.CallType);

        // Get caller name for display
        var callerName = Context.User?.Identity?.Name ?? "Unknown";

        // Send incoming call to receiver
        await Clients.User(request.ReceiverId).SendAsync("incomingCall", new IncomingCallPayload(
            callId,
            request.ChatId,
            callerId,
            callerName,
            request.CallType,
            request.Offer
        ));

        // Confirm call initiated to caller
        await Clients.Caller.SendAsync("callInitiated", new { CallId = callId });
    }

    /// <summary>
    /// Receiver accepts the incoming call
    /// </summary>
    public async Task AcceptCall(AcceptCallRequest request)
    {
        var receiverId = Context.UserIdentifier;
        if (receiverId == null) return;

        var callInfo = _callTracker.GetCallInfo(request.CallId);
        if (callInfo == null)
        {
            await Clients.Caller.SendAsync("callFailed", new CallFailedPayload(request.CallId, "call_not_found"));
            return;
        }

        // Mark call as connected
        _callTracker.MarkCallConnected(request.CallId);

        // Send answer to caller
        await Clients.User(callInfo.CallerId).SendAsync("callAccepted", new CallAcceptedPayload(
            request.CallId,
            request.Answer
        ));
    }

    /// <summary>
    /// Receiver rejects the incoming call
    /// </summary>
    public async Task RejectCall(RejectCallRequest request)
    {
        var receiverId = Context.UserIdentifier;
        if (receiverId == null) return;

        var callInfo = _callTracker.GetCallInfo(request.CallId);
        if (callInfo == null) return;

        // End the call in tracker
        _callTracker.EndCall(request.CallId);

        // Notify caller
        await Clients.User(callInfo.CallerId).SendAsync("callRejected", new CallRejectedPayload(request.CallId));

        // Persist rejected call
        await _callRepo.SaveCallRecordAsync(new CallRecord
        {
            CallId = request.CallId,
            ChatId = callInfo.ChatId,
            CallerId = callInfo.CallerId,
            ReceiverId = callInfo.ReceiverId,
            Type = callInfo.CallType == "video" ? CallType.Video : CallType.Audio,
            Status = CallStatus.Rejected,
            InitiatedAt = callInfo.InitiatedAt,
            EndedAt = DateTime.UtcNow
        });

        // Create call message in chat (WhatsApp style)
        await CreateCallMessage(callInfo.ChatId, callInfo.CallerId, callInfo.ReceiverId, 
            callInfo.CallType == "video" ? CallType.Video : CallType.Audio, 
            CallMessageStatus.Rejected, 0);
    }

    /// <summary>
    /// Caller cancels the call (timeout or user cancelled before answer)
    /// </summary>
    public async Task CancelCall(CancelCallRequest request)
    {
        var callerId = Context.UserIdentifier;
        if (callerId == null) return;

        var callInfo = _callTracker.GetCallInfo(request.CallId);
        if (callInfo == null) return;

        // End the call in tracker
        _callTracker.EndCall(request.CallId);

        // Notify receiver to stop ringing
        await Clients.User(callInfo.ReceiverId).SendAsync("callCancelled", new CallCancelledPayload(request.CallId));

        // Persist as missed call
        await _callRepo.SaveCallRecordAsync(new CallRecord
        {
            CallId = request.CallId,
            ChatId = callInfo.ChatId,
            CallerId = callInfo.CallerId,
            ReceiverId = callInfo.ReceiverId,
            Type = callInfo.CallType == "video" ? CallType.Video : CallType.Audio,
            Status = CallStatus.Missed,
            InitiatedAt = callInfo.InitiatedAt,
            EndedAt = DateTime.UtcNow
        });

        // Create call message in chat (WhatsApp style)
        await CreateCallMessage(callInfo.ChatId, callInfo.CallerId, callInfo.ReceiverId, 
            callInfo.CallType == "video" ? CallType.Video : CallType.Audio, 
            CallMessageStatus.Missed, 0);
    }

    /// <summary>
    /// Either party ends an active call
    /// </summary>
    public async Task EndCall(EndCallRequest request)
    {
        var userId = Context.UserIdentifier;
        if (userId == null) return;

        var callInfo = _callTracker.GetCallInfo(request.CallId);
        if (callInfo == null) return;

        var otherUserId = _callTracker.GetOtherParty(request.CallId, userId);
        
        // Calculate duration
        var durationSeconds = callInfo.ConnectedAt.HasValue
            ? (int)(DateTime.UtcNow - callInfo.ConnectedAt.Value).TotalSeconds
            : 0;

        // End the call in tracker
        _callTracker.EndCall(request.CallId);

        // Notify other party
        if (otherUserId != null)
        {
            await Clients.User(otherUserId).SendAsync("callEnded", new CallEndedPayload(request.CallId, durationSeconds));
        }

        // Persist completed call
        await _callRepo.SaveCallRecordAsync(new CallRecord
        {
            CallId = request.CallId,
            ChatId = callInfo.ChatId,
            CallerId = callInfo.CallerId,
            ReceiverId = callInfo.ReceiverId,
            Type = callInfo.CallType == "video" ? CallType.Video : CallType.Audio,
            Status = CallStatus.Completed,
            InitiatedAt = callInfo.InitiatedAt,
            AnsweredAt = callInfo.ConnectedAt,
            EndedAt = DateTime.UtcNow,
            DurationSeconds = durationSeconds
        });

        // Create call message in chat (WhatsApp style)
        await CreateCallMessage(callInfo.ChatId, callInfo.CallerId, callInfo.ReceiverId, 
            callInfo.CallType == "video" ? CallType.Video : CallType.Audio, 
            CallMessageStatus.Completed, durationSeconds);
    }

    /// <summary>
    /// Create a call message in the chat (like WhatsApp)
    /// </summary>
    private async Task CreateCallMessage(long chatId, string callerId, string receiverId, 
        CallType callType, CallMessageStatus callStatus, int durationSeconds)
    {
        var message = new Message
        {
            ChatId = chatId,
            SenderId = callerId,
            ReceiverId = receiverId,
            Content = GetCallMessageContent(callType, callStatus, durationSeconds),
            Status = MessageStatus.Read, // Call messages are immediately read
            IsCallMessage = true,
            CallType = callType,
            CallDuration = durationSeconds,
            CallStatus = callStatus,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepo.AddAsync(message);
        await _messageRepo.SaveChangesAsync();

        // Notify both parties about the call message
        await _messageNotifier.NotifyMessageReceivedAsync(
            chatId, message.Id, callerId, message.Content, DateTime.UtcNow, 
            (int)message.Status, false, null, null, null,
            true, callType == CallType.Video ? "video" : "audio", durationSeconds, callStatus.ToString().ToLower());
        
        await _messageNotifier.NotifyMessageReceivedAsync(
            chatId, message.Id, callerId, message.Content, DateTime.UtcNow, 
            (int)message.Status, false, null, null, null,
            true, callType == CallType.Video ? "video" : "audio", durationSeconds, callStatus.ToString().ToLower());
    }

    private static string GetCallMessageContent(CallType callType, CallMessageStatus callStatus, int durationSeconds)
    {
        var typeStr = callType == CallType.Video ? "Video" : "Voice";
        return callStatus switch
        {
            CallMessageStatus.Completed => $"{typeStr} call ({durationSeconds}s)",
            CallMessageStatus.Missed => $"Missed {typeStr.ToLower()} call",
            CallMessageStatus.Rejected => $"Declined {typeStr.ToLower()} call",
            _ => $"{typeStr} call"
        };
    }

    /// <summary>
    /// Exchange ICE candidates for WebRTC connection
    /// </summary>
    public async Task SendIceCandidate(IceCandidateRequest request)
    {
        var userId = Context.UserIdentifier;
        if (userId == null) return;

        var otherUserId = _callTracker.GetOtherParty(request.CallId, userId);
        if (otherUserId == null) return;

        await Clients.User(otherUserId).SendAsync("iceCandidate", new IceCandidatePayload(
            request.CallId,
            request.Candidate
        ));
    }

}
