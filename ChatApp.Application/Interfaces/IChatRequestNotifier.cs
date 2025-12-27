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
        Task NotifyRequestAccepted(string fromUserId, long chatId);
        Task NotifyRequestRejected(string fromUserId);
    }

}
