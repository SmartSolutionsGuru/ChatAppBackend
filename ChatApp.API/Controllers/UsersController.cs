using ChatApp.API.Presence;
using ChatApp.Application.Features.Users.Queries.SearchUsers;
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
        private readonly UserConnectionTracker _tracker;

        public UsersController(IMediator mediator, UserConnectionTracker tracker)
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
        public IActionResult GetOnlineUsers()
        {
            var onlineUserIds = _tracker.GetOnlineUserIds();
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
