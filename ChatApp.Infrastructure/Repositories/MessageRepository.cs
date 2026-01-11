using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace ChatApp.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public MessageRepository(ApplicationDbContext db)
            => _db = db;

        public async Task<PaginatedMessagesDto> GetChatMessagesAsync(
            long chatId,
            string userId,
            int pageSize = 30,
            long? beforeMessageId = null)
        {
            var query = _db.Messages
                .Where(m => m.ChatId == chatId);

            // If beforeMessageId is provided, get messages before that ID (older messages)
            if (beforeMessageId.HasValue)
            {
                query = query.Where(m => m.Id < beforeMessageId.Value);
            }

            // Get one extra to check if there are more
            var messages = await query
                .OrderByDescending(m => m.Id)
                .Take(pageSize + 1)
                .Select(m => new 
                {
                    m.Id,
                    m.SenderId,
                    m.Content,
                    m.CreatedAt,
                    Status = (int)m.Status,
                    m.IsVoiceNote,
                    m.VoiceNoteUrl,
                    m.VoiceNoteDuration,
                    m.VoiceNoteWaveform
                })
                .ToListAsync();

            var hasMore = messages.Count > pageSize;
            if (hasMore)
            {
                messages = messages.Take(pageSize).ToList();
            }

            // Reverse to get chronological order (oldest first)
            messages.Reverse();

            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                Status = m.Status,
                IsVoiceNote = m.IsVoiceNote,
                VoiceNoteUrl = m.VoiceNoteUrl,
                VoiceNoteDuration = m.VoiceNoteDuration,
                VoiceNoteWaveform = string.IsNullOrEmpty(m.VoiceNoteWaveform)
                    ? null
                    : JsonSerializer.Deserialize<double[]>(m.VoiceNoteWaveform)
            }).ToList();

            return new PaginatedMessagesDto
            {
                Messages = messageDtos,
                HasMore = hasMore,
                OldestMessageId = messageDtos.FirstOrDefault()?.Id
            };
        }

        public async Task<List<MessageDto>> SearchMessagesAsync(
            long chatId,
            string searchTerm,
            int maxResults = 50)
        {
            var messages = await _db.Messages
                .Where(m => m.ChatId == chatId && 
                            !m.IsVoiceNote &&
                            m.Content.Contains(searchTerm))
                .OrderByDescending(m => m.CreatedAt)
                .Take(maxResults)
                .Select(m => new 
                {
                    m.Id,
                    m.SenderId,
                    m.Content,
                    m.CreatedAt,
                    Status = (int)m.Status,
                    m.IsVoiceNote,
                    m.VoiceNoteUrl,
                    m.VoiceNoteDuration,
                    m.VoiceNoteWaveform
                })
                .ToListAsync();

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                Status = m.Status,
                IsVoiceNote = m.IsVoiceNote,
                VoiceNoteUrl = m.VoiceNoteUrl,
                VoiceNoteDuration = m.VoiceNoteDuration,
                VoiceNoteWaveform = string.IsNullOrEmpty(m.VoiceNoteWaveform)
                    ? null
                    : JsonSerializer.Deserialize<double[]>(m.VoiceNoteWaveform)
            }).ToList();
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(
    long chatId,
    string readerId)
        {
            return await _db.Messages
                .Where(m =>
                    m.ChatId == chatId &&
                    m.ReceiverId == readerId &&
                    m.Status != MessageStatus.Read)
                .ToListAsync();
        }

        public async Task<List<Message>> GetUndeliveredMessagesForUserAsync(string recipientId)
        {
            return await _db.Messages
                .Where(m =>
                    m.ReceiverId == recipientId &&
                    m.Status == MessageStatus.Sent)
                .ToListAsync();
        }

        public Task<Message?> GetByIdAsync(long id)
        {
            return _db.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Message message)
       => await _db.Messages.AddAsync(message);

        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();
    }

}
