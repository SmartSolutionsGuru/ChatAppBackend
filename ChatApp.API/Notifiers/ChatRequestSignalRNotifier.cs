using ChatApp.API.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Notifiers
{
    public class ChatRequestSignalRNotifier : IChatRequestNotifier
    {
        private readonly IHubContext<ChatHub> _hub;

        public ChatRequestSignalRNotifier(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }

        public Task NotifyRequestReceived(string toUserId, long requestId)
            => _hub.Clients.User(toUserId)
                .SendAsync("chatRequestReceived", requestId);

        public Task NotifyRequestAccepted(string fromUserId, long chatId)
            => _hub.Clients.User(fromUserId)
                .SendAsync("chatRequestAccepted", chatId);

        public Task NotifyRequestRejected(string fromUserId)
            => _hub.Clients.User(fromUserId)
                .SendAsync("chatRequestRejected");

      
    }


}
