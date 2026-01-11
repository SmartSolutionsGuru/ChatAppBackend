using ChatApp.API.Presence;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly UserConnectionTracker _tracker;
    private readonly IUserPresenceRepository _presenceRepo;
    private readonly IMessageRepository _messageRepo;
    private readonly IMessageNotifier _messageNotifier;

    public ChatHub(
        UserConnectionTracker tracker,
        IUserPresenceRepository presenceRepo,
        IMessageRepository messageRepo,
        IMessageNotifier messageNotifier)
    {
        _tracker = tracker;
        _presenceRepo = presenceRepo;
        _messageRepo = messageRepo;
        _messageNotifier = messageNotifier;
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

}
