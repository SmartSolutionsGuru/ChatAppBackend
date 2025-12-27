using MediatR;

public record RespondChatRequestCommand(
    long RequestId,
    bool Accept
) : IRequest<long?>;
