using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Features.Users.Queries.GetOnlineUsers;

public class GetOnlineUsersHandler : IRequestHandler<GetOnlineUsersQuery, List<string>>
{
    private readonly IUserConnectionTracker _tracker;

    public GetOnlineUsersHandler(IUserConnectionTracker tracker)
    {
        _tracker = tracker;
    }

    public Task<List<string>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        var onlineUsers = _tracker.GetOnlineUserIds().ToList();
        return Task.FromResult(onlineUsers);
    }
}
