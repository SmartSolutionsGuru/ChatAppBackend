using ChatApp.Application.Features.ChatRequests.Commands.SendChatRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/chat-requests")]
public class ChatRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatRequestsController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Send(SendChatRequestCommand cmd)
    {
        var id = await _mediator.Send(cmd);
        return Ok(new { requestId = id });
    }

    [HttpPost("{id}/respond")]
    public async Task<IActionResult> Respond(
        long id,
        [FromBody] bool accept)
    {
        var chatId = await _mediator.Send(
            new RespondChatRequestCommand(id, accept));

        return Ok(new { chatId });
    }
}
