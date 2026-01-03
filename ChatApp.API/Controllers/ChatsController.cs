using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Application.Features.Chat.Queries.GetMyChats;
using ChatApp.Application.Features.Chat.Queries.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chats")]
    public class ChatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMyChats()
        {
            var chats = await _mediator.Send(new GetMyChatsQuery());
            return Ok(chats);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(long chatId)
        {
            var messages = await _mediator.Send(
                new GetChatMessagesQuery(chatId));

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send(SendMessageCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new { messageId = id });
        }

    }
}
