using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Chat.Queries.DeliverMessage
{
    public class DeliverMessageHandler
    : IRequestHandler<DeliverMessageCommand>
    {
        private readonly IMessageRepository _messages;
        private readonly ICurrentUserService _currentUser;
        private readonly IMessageNotifier _notifier;

        public DeliverMessageHandler(
            IMessageRepository messages,
            ICurrentUserService currentUser,
            IMessageNotifier notifier)
        {
            _messages = messages;
            _currentUser = currentUser;
            _notifier = notifier;
        }

        public async Task Handle(
            DeliverMessageCommand request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;

            var message = await _messages.GetByIdAsync(request.MessageId)
                ?? throw new InvalidOperationException("Message not found");

            // Only receiver can mark as delivered
            if (message.ReceiverId != userId)
                return;

            // Idempotent (VERY important)
            if (message.Status != MessageStatus.Sent)
                return;

            message.Status = MessageStatus.Delivered;

            await _messages.SaveChangesAsync();

            // notify sender (all tabs)
            await _notifier.NotifyMessageDelivered(
                message.ChatId,
                message.Id
            );
        }
    }
}
