using ChatApp.Application.DTOs.Calls;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class CallRepository : ICallRepository
    {
        private readonly ApplicationDbContext _context;

        public CallRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CallRecord> SaveCallRecordAsync(CallRecord callRecord)
        {
            _context.CallRecords.Add(callRecord);
            await _context.SaveChangesAsync();
            return callRecord;
        }

        public async Task<List<CallHistoryItem>> GetCallHistoryAsync(string userId, int page = 1, int pageSize = 20)
        {
            var calls = await _context.CallRecords
                .Where(c => c.CallerId == userId || c.ReceiverId == userId)
                .OrderByDescending(c => c.InitiatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    Call = c,
                    OtherUserId = c.CallerId == userId ? c.ReceiverId : c.CallerId,
                    IsOutgoing = c.CallerId == userId
                })
                .ToListAsync();

            // Get user names
            var otherUserIds = calls.Select(c => c.OtherUserId).Distinct().ToList();
            var userNames = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.UserName ?? "Unknown");

            return calls.Select(c => new CallHistoryItem(
                c.Call.Id,
                c.Call.CallId,
                c.Call.ChatId,
                c.OtherUserId,
                userNames.GetValueOrDefault(c.OtherUserId, "Unknown"),
                c.Call.Type.ToString().ToLower(),
                c.Call.Status.ToString().ToLower(),
                c.IsOutgoing,
                c.Call.InitiatedAt,
                c.Call.AnsweredAt,
                c.Call.EndedAt,
                c.Call.DurationSeconds
            )).ToList();
        }

        public async Task<List<CallHistoryItem>> GetChatCallHistoryAsync(long chatId, string currentUserId, int page = 1, int pageSize = 20)
        {
            var calls = await _context.CallRecords
                .Where(c => c.ChatId == chatId)
                .OrderByDescending(c => c.InitiatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    Call = c,
                    OtherUserId = c.CallerId == currentUserId ? c.ReceiverId : c.CallerId,
                    IsOutgoing = c.CallerId == currentUserId
                })
                .ToListAsync();

            var otherUserIds = calls.Select(c => c.OtherUserId).Distinct().ToList();
            var userNames = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.UserName ?? "Unknown");

            return calls.Select(c => new CallHistoryItem(
                c.Call.Id,
                c.Call.CallId,
                c.Call.ChatId,
                c.OtherUserId,
                userNames.GetValueOrDefault(c.OtherUserId, "Unknown"),
                c.Call.Type.ToString().ToLower(),
                c.Call.Status.ToString().ToLower(),
                c.IsOutgoing,
                c.Call.InitiatedAt,
                c.Call.AnsweredAt,
                c.Call.EndedAt,
                c.Call.DurationSeconds
            )).ToList();
        }

        public async Task<CallRecord?> GetByCallIdAsync(Guid callId)
        {
            return await _context.CallRecords
                .FirstOrDefaultAsync(c => c.CallId == callId);
        }
    }
}
