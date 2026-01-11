using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;

public class RespondChatRequestHandler
    : IRequestHandler<RespondChatRequestCommand, long?>
{
    private readonly IChatRequestRepository _requests;
    private readonly IChatRepository _chats;
    private readonly ICurrentUserService _currentUser;
    private readonly IChatRequestNotifier _notifier;
    private readonly IUserSearchRepository _users;

    public RespondChatRequestHandler(
        IChatRequestRepository requests,
        IChatRepository chats,
        ICurrentUserService currentUser,
        IChatRequestNotifier notifier,
        IUserSearchRepository users)
    {
        _requests = requests;
        _chats = chats;
        _currentUser = currentUser;
        _notifier = notifier;
        _users = users;
    }

    public async Task<long?> Handle(
     RespondChatRequestCommand request,
     CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var chatRequest = await _requests.GetByIdAsync(request.RequestId)
            ?? throw new InvalidOperationException("Request not found.");

        if (chatRequest.ToUserId != userId)
            throw new UnauthorizedAccessException();

        if (chatRequest.Status != ChatRequestStatus.Pending)
            throw new InvalidOperationException("Request already handled.");

        if (!request.Accept)
        {
            chatRequest.Reject();
            await _requests.SaveChangesAsync();

            await _notifier.NotifyRequestRejected(chatRequest.FromUserId);
            return null; // ✅ RETURN
        }

        chatRequest.Accept();

        var chat = await _chats.GetOrCreateAsync(
            chatRequest.FromUserId,
            chatRequest.ToUserId);

        await _requests.SaveChangesAsync();
        await _chats.SaveChangesAsync();

        // Get usernames for notifications
        var acceptedByUserName = await _users.GetUserNameByIdAsync(userId) ?? "User";
        var fromUserName = await _users.GetUserNameByIdAsync(chatRequest.FromUserId) ?? "User";

        // Notify the original sender that their request was accepted
        await _notifier.NotifyRequestAccepted(
            chatRequest.FromUserId,
            chat.Id,
            userId,
            acceptedByUserName);

        // Notify the accepter about the new chat (for their sidebar)
        await _notifier.NotifyChatCreated(
            userId,
            chat.Id,
            chatRequest.FromUserId,
            fromUserName);

        return chat.Id; // ✅ RETURN
    }

}
