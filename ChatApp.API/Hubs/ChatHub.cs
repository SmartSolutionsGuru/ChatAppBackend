using ChatApp.API.Presence;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly UserConnectionTracker _tracker;
    private readonly IUserPresenceRepository _presenceRepo;

    public ChatHub(
        UserConnectionTracker tracker,
        IUserPresenceRepository presenceRepo)
    {
        _tracker = tracker;
        _presenceRepo = presenceRepo;
    }
    public async Task Typing(long chatId)
    {
        var userId = Context.UserIdentifier;

        if (userId == null) return;

        await Clients
            .Group(chatId.ToString())
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
            .Group(chatId.ToString())
            .SendAsync("stopTyping", new
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
        if (userId == null) return;

        var isFirstConnection = _tracker.UserConnected(userId);

        if (isFirstConnection)
        {
            await Clients.All.SendAsync("userOnline", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId == null) return;

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

        await base.OnDisconnectedAsync(exception);
    }
}
