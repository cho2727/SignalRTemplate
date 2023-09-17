using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRTemplate.Authorization;
using SignalRTemplate.SignalR;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddJsonProtocol(option =>
{
    option.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthorizationHandler, ChannelHubAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ChannelHubAuthorizationPolicy", policy =>
    {
        policy.Requirements.Add(new ChannelHubAuthorizationRequirement());
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

//app.MapControllers();
app.MapHub<ChannelHub>("/channel", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

var pushRouter = app.MapGroup("push").WithOpenApi();

pushRouter.MapPost("all",
    async ([FromBody] ChannelMessageInfo messageInfo, IHubContext<ChannelHub> hubContext)
    => await hubContext.Clients.Group(messageInfo.Id).SendAsync("receiveMessage", messageInfo.Message));

pushRouter.MapPost("user",
    async ([FromBody] ChannelMessageInfo messageInfo, IHubContext<ChannelHub> hubContext)
    => await hubContext.Clients.Group(messageInfo.Id).SendAsync("receiveMessage", messageInfo.Message));

app.Run();

class ChannelMessageInfo
{
    public string Message { get; set; }
    public string Id { get; set; }
}
