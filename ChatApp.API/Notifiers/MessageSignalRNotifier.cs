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
            MessageStatus status,
            bool isVoiceNote = false,
            string? voiceNoteUrl = null,
            double? voiceNoteDuration = null,
            double[]? voiceNoteWaveform = null)
        {
            var payload = new
            {
                messageId,
                chatId,
                senderId,
                receiverId,
                content,
                createdAt,
                status = (int)status,
                isVoiceNote,
                voiceNoteUrl,
                voiceNoteDuration,
                voiceNoteWaveform
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
        long messageId,
        string senderId)
        {
            await _hub.Clients
                .User(senderId)
                .SendAsync("messageDelivered", new
                {
                    chatId,
                    messageId
                });
        }
    }
}
