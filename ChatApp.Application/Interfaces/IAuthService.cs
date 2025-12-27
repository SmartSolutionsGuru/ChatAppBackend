using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);

    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string password, string? displayName);
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshAsync(string refreshToken);
    }
}
