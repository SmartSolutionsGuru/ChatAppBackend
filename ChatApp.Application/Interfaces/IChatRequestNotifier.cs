using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRequestNotifier
    {
        Task NotifyRequestReceived(string toUserId, long requestId);
        Task NotifyRequestAccepted(string fromUserId, long chatId, string acceptedByUserId, string acceptedByUserName);
        Task NotifyRequestRejected(string fromUserId);
        Task NotifyChatCreated(string toUserId, long chatId, string otherUserId, string otherUserName);
    }

}
