using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Application.Interfaces;
using MediatR;

namespace ChatApp.Application.Features.Chat.Queries.SearchMessages;

public class SearchMessagesHandler : IRequestHandler<SearchMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepo;

    public SearchMessagesHandler(IMessageRepository messageRepo)
    {
        _messageRepo = messageRepo;
    }

    public async Task<List<MessageDto>> Handle(
        SearchMessagesQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return new List<MessageDto>();

        return await _messageRepo.SearchMessagesAsync(
            request.ChatId,
            request.SearchTerm,
            request.MaxResults);
    }
}
