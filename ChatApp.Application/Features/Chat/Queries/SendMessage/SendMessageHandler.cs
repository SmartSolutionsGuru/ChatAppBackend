using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.SendMessage
{
    public class SendMessageHandler
    : IRequestHandler<SendMessageCommand, long>
    {
        private readonly IMessageRepository _messages;
        private readonly IChatRepository _chats;
        private readonly ICurrentUserService _currentUser;
        private readonly IMessageNotifier _notifier;

        public SendMessageHandler(
            IMessageRepository messages,
            IChatRepository chats,
            ICurrentUserService currentUser,
            IMessageNotifier notifier)
        {
            _messages = messages;
            _chats = chats;
            _currentUser = currentUser;
            _notifier = notifier;
        }

        public async Task<long> Handle(
            SendMessageCommand request,
            CancellationToken ct)
        {
            var senderId = _currentUser.UserId;

            var chat = await _chats.GetByIdAsync(request.ChatId)
                ?? throw new InvalidOperationException("Chat not found");

            if (chat.User1Id != senderId && chat.User2Id != senderId)
                throw new UnauthorizedAccessException();

            var receiverId =
                chat.User1Id == senderId
                    ? chat.User2Id
                    : chat.User1Id;

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = request.Content,
                Status = MessageStatus.Sent
            };

            await _messages.AddAsync(message);
            await _messages.SaveChangesAsync();

            await _notifier.NotifyMessageSent(
                message.Id,
                chat.Id,
                senderId,
                receiverId,
                message.Content,
                message.CreatedAt,
                message.Status
            );

            return message.Id;
        }
    }
}
