using ChatApp.API.Extensions;
using ChatApp.API.Notifiers;
using ChatApp.API.Presence;
using ChatApp.Application.Features.ChatRequests.Commands.SendChatRequest;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Identity;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssembly(typeof(SendChatRequestCommand).Assembly));
builder.Services.AddSignalR();
builder.Services.AddSingleton<UserConnectionTracker>();
builder.Services.AddScoped<IChatRequestNotifier, ChatRequestSignalRNotifier>();
builder.Services.AddScoped<IUserPresenceRepository, UserPresenceRepository>();
builder.Services.AddScoped<IMessageNotifier, MessageSignalRNotifier>();
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseCors("AllowAngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatApp.API.Hubs.ChatHub>("/hubs/chat");
app.Run();
