using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Features.ChatRequests.Commands.SendChatRequest;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using MediatR;

public class SendChatRequestHandler
    : IRequestHandler<SendChatRequestCommand, long>
{
    private readonly IChatRequestRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IChatRequestNotifier _notifier; // ✅ ADD THIS
    public SendChatRequestHandler(
        IChatRequestRepository repo,
        ICurrentUserService currentUser,
        IChatRequestNotifier notifier) // ✅ INJECT THIS
    {
        _repo = repo;
        _currentUser = currentUser;
        _notifier = notifier; // ✅ ASSIGN
    }

    public async Task<long> Handle(
     SendChatRequestCommand request,
     CancellationToken ct)
    {
        var fromUserId = _currentUser.UserId;

        if (fromUserId == request.ToUserId)
            throw new InvalidOperationException("Cannot send request to yourself.");

        var exists = await _repo.ExistsPendingAsync(fromUserId, request.ToUserId);
        if (exists)
            throw new InvalidOperationException("Chat request already exists.");

        var chatRequest = new ChatRequest
        {
            FromUserId = fromUserId,
            ToUserId = request.ToUserId
        };

        await _repo.AddAsync(chatRequest);
        await _repo.SaveChangesAsync();

        await _notifier.NotifyRequestReceived(
            request.ToUserId,
            chatRequest.Id);

        return chatRequest.Id; // ✅ REQUIRED
    }

}
