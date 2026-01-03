using ChatApp.API.Hubs;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Notifiers
{
    public class MessageSignalRNotifier : IMessageNotifier
    {
        private readonly IHubContext<ChatHub> _hub;

        public MessageSignalRNotifier(IHubContext<ChatHub> hub)
            => _hub = hub;

        public async Task NotifyMessageSent(
            long messageId,
            long chatId,
            string senderId,
            string receiverId,
            string content,
            DateTime createdAt,
            MessageStatus status)
        {
            var payload = new
            {
                messageId,
                chatId,
                senderId,
                receiverId,
                content,
                createdAt,
                status = status.ToString()
            };

            await _hub.Clients.User(senderId)
                .SendAsync("messageReceived", payload);

            await _hub.Clients.User(receiverId)
                .SendAsync("messageReceived", payload);
        }

        public async Task NotifyMessagesRead(
    long chatId,
    List<long> messageIds,
    string senderId)
        {
            await _hub.Clients.User(senderId)
                .SendAsync("messagesRead", new
                {
                    chatId,
                    messageIds
                });
        }

        public async Task NotifyMessageDelivered(
        long chatId,
        long messageId)
        {
            await _hub.Clients
                .Group(chatId.ToString())
                .SendAsync("messageDelivered", new
                {
                    chatId,
                    messageId
                });
        }
    }
}
