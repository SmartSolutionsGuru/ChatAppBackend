using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IMessageNotifier
    {
    Task NotifyMessagesRead(
    long chatId,
    List<long> messageIds,
    string senderId);
        Task NotifyMessageSent(
            long messageId,
            long chatId,
            string senderId,
            string receiverId,
            string content,
            DateTime createdAt,
            MessageStatus status);
        Task NotifyMessageDelivered(long chatId, long messageId);
    }
}
