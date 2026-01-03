using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.ReadMessages
{
    public class ReadMessagesHandler
    : IRequestHandler<ReadMessagesCommand>
    {
        private readonly IMessageRepository _messages;
        private readonly IChatRepository _chats;
        private readonly ICurrentUserService _currentUser;
        private readonly IMessageNotifier _notifier;

        public ReadMessagesHandler(
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

        public async Task Handle(
            ReadMessagesCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var chat = await _chats.GetByIdAsync(request.ChatId)
                ?? throw new InvalidOperationException("Chat not found");

            // Only participants
            if (chat.User1Id != userId && chat.User2Id != userId)
                throw new UnauthorizedAccessException();

            var messages = await _messages.GetUnreadMessagesAsync(
                request.ChatId,
                userId);

            if (!messages.Any())
                return;

            foreach (var m in messages)
                m.Status = MessageStatus.Read;

            await _messages.SaveChangesAsync();

            var otherUserId =
                chat.User1Id == userId
                    ? chat.User2Id
                    : chat.User1Id;

            await _notifier.NotifyMessagesRead(
                request.ChatId,
                messages.Select(m => m.Id).ToList(),
                otherUserId
            );
        }
    }
}
