using Azure.Core;
using ChatApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChatApp.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services,
        IConfiguration config)
    {
        var jwt = config.GetSection("Jwt").Get<JwtOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromSeconds(10)
                };

                // SignalR support
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}


//CLIENT
//  |
//  |  WebSocket handshake request
//  |  Token in query string
//  v
//┌──────────────────────────────┐
//│ ASP.NET Middleware Pipeline  │
//└──────────────────────────────┘
//          |
//          v
//┌──────────────────────────────┐
//│ UseAuthentication middleware │
//└──────────────────────────────┘
//          |
//          v
//┌─────────────────────────────────────────┐
//│ JwtBearerHandler starts authentication  │
//└─────────────────────────────────────────┘
//          |
//          v
//┌────────────────────────────────────────────────────┐
//│ 🔥 OnMessageReceived EVENT FIRES                    │
//│                                                    │
//│ 1. Read query string                               │
//│    access_token = context.Request.Query[...]       │
//│                                                    │
//│ 2. Check path == /hubs/chat                        │
//│                                                    │
//│ 3. Inject token:                                  │
//│    context.Token = access_token                   │
//└────────────────────────────────────────────────────┘
//          |
//          v
//┌─────────────────────────────────────────┐
//│ JWT handler NOW HAS A TOKEN              │
//└─────────────────────────────────────────┘
//          |
//          v
//┌─────────────────────────────────────────┐
//│ TokenValidationParameters applied       │
//│ - Issuer                                │
//│ - Audience                              │
//│ - Expiry                                │
//│ - Signature                             │
//└─────────────────────────────────────────┘
//          |
//          v
//┌─────────────────────────────────────────┐
//│ ClaimsPrincipal created                 │
//│ Context.User available in ChatHub       │
//└─────────────────────────────────────────┘
//          |
//          v
//┌──────────────────────────────┐
//│ ChatHub.OnConnectedAsync()   │
//└──────────────────────────────┘


//REQUEST COMES IN
//                      |
//                      v
//        ┌────────────────────────────┐
//        │ UseAuthentication middleware│
//        └────────────────────────────┘
//                      |
//                      v
//        ┌────────────────────────────┐
//        │ JwtBearerHandler starts     │
//        └────────────────────────────┘
//                      |
//                      v
//        ┌─────────────────────────────────────┐
//        │ OnMessageReceived EVENT              │
//        │                                     │
//        │ If HTTP:                            │
//        │   - Query token ❌                  │
//        │   - Do nothing                     │
//        │                                     │
//        │ If SignalR:                         │
//        │   - Token in query ✅               │
//        │   - context.Token = token          │
//        └─────────────────────────────────────┘
//                      |
//                      v
//        ┌────────────────────────────┐
//        │ Token validation rules     │
//        │ (issuer, expiry, signature)│
//        └────────────────────────────┘
//                      |
//                      v
//        ┌────────────────────────────┐
//        │ ClaimsPrincipal created    │
//        │ HttpContext.User populated │
//        └────────────────────────────┘
//                      |
//                      v
//        ┌────────────────────────────┐
//        │ Controller / ChatHub runs  │
//        └────────────────────────────┘
