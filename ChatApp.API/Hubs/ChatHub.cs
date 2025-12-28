using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task WhoAmI()
    {
        await Clients.Caller.SendAsync(
            "whoAmI",
            Context.UserIdentifier
        );
    }
}
