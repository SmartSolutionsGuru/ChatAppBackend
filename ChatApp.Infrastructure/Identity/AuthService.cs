using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly JwtOptions _opt;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db,
        IJwtTokenService jwt,
        IOptions<JwtOptions> opt)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _opt = opt.Value;
    }

    public async Task<AuthResponse> RegisterAsync(string email, string password, string? displayName)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null) throw new InvalidOperationException("Email already registered.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" | ", result.Errors.Select(e => e.Description)));

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email)
                   ?? throw new InvalidOperationException("Invalid credentials.");

        var ok = await _userManager.CheckPasswordAsync(user, password);
        if (!ok) throw new InvalidOperationException("Invalid credentials.");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
        if (stored == null || !stored.IsActive)
            throw new InvalidOperationException("Invalid refresh token.");

        var user = await _userManager.FindByIdAsync(stored.UserId)
                   ?? throw new InvalidOperationException("User not found.");

        // revoke old token (rotate)
        stored.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    private async Task<AuthResponse> IssueTokensAsync(ApplicationUser user)
    {
        var (access, expires) = _jwt.CreateAccessToken(user.Id, user.Email ?? user.UserName ?? "");
        var refresh = _jwt.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refresh,
            ExpiresAt = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays)
        });

        await _db.SaveChangesAsync();

        return new AuthResponse(access, refresh, expires);
    }
}
