using MediatR;

namespace ChatApp.Application.Features.Users.Queries.GetOnlineUsers;

public record GetOnlineUsersQuery() : IRequest<List<string>>;
