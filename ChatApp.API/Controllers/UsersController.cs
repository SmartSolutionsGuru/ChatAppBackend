using ChatApp.Application.Features.Users.Queries.GetOnlineUsers;
using ChatApp.Application.Features.Users.Queries.SearchUsers;
using ChatApp.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserConnectionTracker _tracker;

        public UsersController(IMediator mediator, IUserConnectionTracker tracker)
        {
            _mediator = mediator;
            _tracker = tracker;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<UserSearchItemDto>());

            var users = await _mediator.Send(new SearchUsersQuery(term));
            return Ok(users);
        }

        [HttpGet("online")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            var onlineUserIds = await _mediator.Send(new GetOnlineUsersQuery());
            return Ok(onlineUserIds);
        }

        [HttpGet("{userId}/online")]
        public IActionResult IsUserOnline(string userId)
        {
            var isOnline = _tracker.IsOnline(userId);
            return Ok(new { userId, isOnline });
        }
    }

}
