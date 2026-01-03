using ChatApp.Application.Features.Chat.Queries.DeliverMessage;
using ChatApp.Application.Features.Chat.Queries.ReadMessages;
using ChatApp.Application.Features.Chat.Queries.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
            => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Send(SendMessageCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new { messageId = id });
        }

        [HttpPost("{id}/delivered")]
        public async Task<IActionResult> Delivered(long id)
        {
            await _mediator.Send(new DeliverMessageCommand(id));
            return Ok();
        }

        [HttpPost("chat/{chatId}/read")]
        public async Task<IActionResult> MarkRead(long chatId)
        {
            await _mediator.Send(new ReadMessagesCommand(chatId));
            return Ok();
        }
    }
}
