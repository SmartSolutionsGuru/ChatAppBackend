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

    public RespondChatRequestHandler(
        IChatRequestRepository requests,
        IChatRepository chats,
        ICurrentUserService currentUser,
        IChatRequestNotifier notifier)
    {
        _requests = requests;
        _chats = chats;
        _currentUser = currentUser;
        _notifier = notifier;
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

        await _notifier.NotifyRequestAccepted(
            chatRequest.FromUserId,
            chat.Id);

        return chat.Id; // ✅ RETURN
    }

}
