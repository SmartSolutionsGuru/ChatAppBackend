using ChatApp.Application.Common.Interfaces;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure.Identity;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Presence;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<JwtOptions>(
            config.GetSection("Jwt")
        );
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddHttpContextAccessor();
        
        // Core services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Repositories
        services.AddScoped<IChatRequestRepository, ChatRequestRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IChatReadRepository, ChatReadRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserSearchRepository, UserSearchRepository>();
        services.AddScoped<IUserPresenceRepository, UserPresenceRepository>();
        services.AddScoped<ICallRepository, CallRepository>();
        
        // Presence tracking (Singleton for connection tracking across requests)
        services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();
        
        // Active call tracking (Singleton for in-memory call state)
        services.AddSingleton<IActiveCallTracker, ActiveCallTracker>();
        
        return services;
    }
}
