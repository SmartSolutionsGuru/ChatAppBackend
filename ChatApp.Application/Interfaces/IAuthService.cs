using ChatApp.Application.DTOs.Auth;

namespace ChatApp.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(string email, string password, string? displayName);
    Task<AuthResponse> LoginAsync(string email, string password);
    Task<AuthResponse> RefreshAsync(string refreshToken);
}
