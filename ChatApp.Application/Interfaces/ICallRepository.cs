using ChatApp.Application.DTOs.Calls;
using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface ICallRepository
    {
        /// <summary>
        /// Save a call record (final outcome only)
        /// </summary>
        Task<CallRecord> SaveCallRecordAsync(CallRecord callRecord);

        /// <summary>
        /// Get call history for a user
        /// </summary>
        Task<List<CallHistoryItem>> GetCallHistoryAsync(string userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Get call history for a specific chat
        /// </summary>
        Task<List<CallHistoryItem>> GetChatCallHistoryAsync(long chatId, string currentUserId, int page = 1, int pageSize = 20);

        /// <summary>
        /// Get a specific call record by CallId (GUID)
        /// </summary>
        Task<CallRecord?> GetByCallIdAsync(Guid callId);
    }
}
