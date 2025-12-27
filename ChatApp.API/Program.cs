using ChatApp.API.Extensions;
using ChatApp.API.Notifiers;
using ChatApp.Application.Features.ChatRequests.Commands.SendChatRequest;
using ChatApp.Application.Interfaces;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SendChatRequestCommand).Assembly));
builder.Services.AddSignalR();
builder.Services.AddScoped<IChatRequestNotifier, ChatRequestSignalRNotifier>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatApp.API.Hubs.ChatHub>("/hubs/chat");

app.Run();
