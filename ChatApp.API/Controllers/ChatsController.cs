using ChatApp.Application.Features.Chat.Queries.GetChatMessages;
using ChatApp.Application.Features.Chat.Queries.GetMyChats;
using ChatApp.Application.Features.Chat.Queries.SendMessage;
using ChatApp.Application.Interfaces;
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
        private readonly IMessageRepository _messageRepo;

        public ChatsController(IMediator mediator, IMessageRepository messageRepo)
        {
            _mediator = mediator;
            _messageRepo = messageRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyChats()
        {
            var chats = await _mediator.Send(new GetMyChatsQuery());
            return Ok(chats);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(
            long chatId,
            [FromQuery] int pageSize = 30,
            [FromQuery] long? beforeMessageId = null)
        {
            var result = await _mediator.Send(
                new GetChatMessagesQuery(chatId, pageSize, beforeMessageId));

            return Ok(result);
        }

        [HttpGet("{chatId}/messages/search")]
        public async Task<IActionResult> SearchMessages(
            long chatId,
            [FromQuery] string q,
            [FromQuery] int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new List<MessageDto>());

            var results = await _messageRepo.SearchMessagesAsync(chatId, q, maxResults);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Send(SendMessageCommand cmd)
        {
            var id = await _mediator.Send(cmd);
            return Ok(new { messageId = id });
        }

    }
}
