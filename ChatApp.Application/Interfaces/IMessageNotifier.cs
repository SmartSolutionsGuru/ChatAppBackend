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
            MessageStatus status,
            bool isVoiceNote = false,
            string? voiceNoteUrl = null,
            double? voiceNoteDuration = null,
            double[]? voiceNoteWaveform = null);
        Task NotifyMessageDelivered(long chatId, long messageId, string senderId);
        
        // Notify about a message including call message fields
        Task NotifyMessageReceivedAsync(
            long chatId,
            long messageId,
            string senderId,
            string content,
            DateTime createdAt,
            int status,
            bool isVoiceNote,
            string? voiceNoteUrl,
            double? voiceNoteDuration,
            double[]? voiceNoteWaveform,
            bool isCallMessage = false,
            string? callType = null,
            int? callDuration = null,
            string? callStatus = null);
    }
}
