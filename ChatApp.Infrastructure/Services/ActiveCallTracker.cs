using ChatApp.Application.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ChatApp.Infrastructure.Services
{
    /// <summary>
    /// In-memory tracker for active calls (ephemeral state)
    /// Thread-safe implementation using ConcurrentDictionary
    /// </summary>
    public class ActiveCallTracker : IActiveCallTracker
    {
        // CallId -> CallInfo
        private readonly ConcurrentDictionary<Guid, ActiveCallInfo> _activeCalls = new();
        
        // UserId -> CallId (for quick user lookup)
        private readonly ConcurrentDictionary<string, Guid> _userToCall = new();

        public void RegisterPendingCall(Guid callId, long chatId, string callerId, string receiverId, string callType)
        {
            var callInfo = new ActiveCallInfo
            {
                CallId = callId,
                ChatId = chatId,
                CallerId = callerId,
                ReceiverId = receiverId,
                CallType = callType,
                InitiatedAt = DateTime.UtcNow,
                IsConnected = false
            };

            _activeCalls.TryAdd(callId, callInfo);
            _userToCall.TryAdd(callerId, callId);
            // Don't add receiver yet - they're just ringing, not in call
        }

        public void MarkCallConnected(Guid callId)
        {
            if (_activeCalls.TryGetValue(callId, out var callInfo))
            {
                callInfo.IsConnected = true;
                callInfo.ConnectedAt = DateTime.UtcNow;
                
                // Now add receiver to active call users
                _userToCall.TryAdd(callInfo.ReceiverId, callId);
            }
        }

        public void EndCall(Guid callId)
        {
            if (_activeCalls.TryRemove(callId, out var callInfo))
            {
                _userToCall.TryRemove(callInfo.CallerId, out _);
                _userToCall.TryRemove(callInfo.ReceiverId, out _);
            }
        }

        public bool IsUserInCall(string userId)
        {
            if (!_userToCall.TryGetValue(userId, out var callId))
                return false;

            // Verify the call is actually connected
            if (_activeCalls.TryGetValue(callId, out var callInfo))
            {
                return callInfo.IsConnected;
            }

            return false;
        }

        public bool HasPendingCall(string userId)
        {
            // Check if user has an incoming call that hasn't been answered yet
            return _activeCalls.Values.Any(c => 
                c.ReceiverId == userId && !c.IsConnected);
        }

        public string? GetOtherParty(Guid callId, string userId)
        {
            if (_activeCalls.TryGetValue(callId, out var callInfo))
            {
                return callInfo.CallerId == userId 
                    ? callInfo.ReceiverId 
                    : callInfo.CallerId;
            }
            return null;
        }

        public ActiveCallInfo? GetCallInfo(Guid callId)
        {
            _activeCalls.TryGetValue(callId, out var callInfo);
            return callInfo;
        }

        public ActiveCallInfo? GetPendingCallForReceiver(string receiverId)
        {
            return _activeCalls.Values.FirstOrDefault(c => 
                c.ReceiverId == receiverId && !c.IsConnected);
        }
    }
}
