using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRTemplate.Extensions;

namespace SignalRTemplate.SignalR;

[Authorize("ChannelHubAuthorizationPolicy")]
public class ChannelHub : Hub
{
    private readonly ILogger<ChannelHub> _logger;

    public ChannelHub(ILogger<ChannelHub> logger)
    {
        _logger = logger;
    }

    [HubMethodName("sendMessage")]
    public async Task SendMessage(string channelId, string message)
    {
        _logger.LogInformation($"{Context.User}");

        await Clients.OthersInGroup(channelId).SendAsync("receiveMessage", message);
    }


    // 하나는 channel 을 관리하기 위한 group
    // 다른 하나는 client 를 관리하기 위한 group 
    [HubMethodName("create")]
    public async Task<Create.Response> CreateChannel()
    {
        var channelId = Guid.NewGuid().ToAlphaNumeric();
        var joinId = Guid.NewGuid().ToAlphaNumeric();
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        await Groups.AddToGroupAsync(Context.ConnectionId, joinId);
        return new Create.Response { ChannelId = channelId, JoinId = joinId };
    }


    // 하나는 channel 을 관리하기 위한 group
    // 다른 하나는 client 를 관리하기 위한 group 
    [HubMethodName("join")]
    public async Task<Join.Response> JoinChannel(string channelId)
    {
        var joinId = Guid.NewGuid().ToAlphaNumeric();
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        await Groups.AddToGroupAsync(Context.ConnectionId, joinId);
        return new Join.Response { JoinId = joinId };
    }

    [HubMethodName("left")]
    public async Task LeftChannel(string channelId, string joinId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, joinId);
    }
}
