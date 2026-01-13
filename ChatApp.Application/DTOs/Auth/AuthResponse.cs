namespace ChatApp.Application.DTOs.Auth;

public record AuthResponse(
    string AccessToken, 
    string RefreshToken, 
    DateTime AccessTokenExpiresAt
);
