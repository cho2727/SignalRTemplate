// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using SignalRClient.SignalR;
Console.WriteLine("Hello, World!");

HubConnection InitSignalRClient()
{
    var url = $"https://localhost:5004/channel";
    var connection = new HubConnectionBuilder()
        .WithUrl(url, options =>
        {
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
            options.SkipNegotiation = true;
        })
        .WithAutomaticReconnect()
        .Build();
    return connection;
}

var connection1 = InitSignalRClient();
await connection1.StartAsync();
var createResponse = await connection1.InvokeAsync<Create.Response>("create");
connection1.On<string>("receiveMessage", message => Console.WriteLine($"connection1 receive : {message}"));


var connection2 = InitSignalRClient();
await connection2.StartAsync();
var joinResponse = await connection2.InvokeAsync<Create.Response>("join", createResponse.ChannelId);
connection2.On<string>("receiveMessage", message => Console.WriteLine($"connection2 receive : {message}"));

await connection2.SendAsync("sendMessage", createResponse.ChannelId, "hello i'm connection2");
await connection1.SendAsync("sendMessage", createResponse.ChannelId, "hello i'm connection1");

Console.ReadLine();