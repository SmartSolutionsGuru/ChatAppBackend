using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using MediatR;

namespace ChatApp.Application.Features.Chat.Queries.SearchMessages;

public record SearchMessagesQuery(
    long ChatId,
    string SearchTerm,
    int MaxResults = 50
) : IRequest<List<MessageDto>>;
