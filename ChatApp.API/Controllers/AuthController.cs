using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var res = await _auth.RegisterAsync(req.Email, req.Password, req.DisplayName);
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var res = await _auth.LoginAsync(req.Email, req.Password);
        return Ok(res);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest req)
    {
        var res = await _auth.RefreshAsync(req.RefreshToken);
        return Ok(res);
    }
}

public record RegisterRequest(string Email, string Password, string? DisplayName);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
