// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using SignalRClient.Extensions;
using SignalRClient.SignalR;
Console.WriteLine("Hello, World!");

async Task<HubConnection> InitSignalRClient()
{

    var identityUrl = $"https://localhost:7093/user/login";
    HttpClient client = new HttpClient();
    var request = new
    {
        Id = "test001",
        Password = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4",
    };
    var contents = new StringContent(request.SerializeToJson(), System.Text.Encoding.UTF8, "application/json");
    var response = await client.PostAsync(identityUrl, contents);
    string token = string.Empty;
    if (response.IsSuccessStatusCode)
    {
        var loginResponse = await response.Content.ReadAsAsync<Login.Response>();
        token = loginResponse?.AccessToken ?? "";
    }


    var url = $"https://localhost:5004/channel";
    var connection = new HubConnectionBuilder()
        .WithUrl(url, options =>
        {
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
            options.SkipNegotiation = true;
            options.Headers.Add("Authorization", $"bearer {token}");
        })
        .WithAutomaticReconnect()
        .Build();
    return connection;
}

var connection1 = await InitSignalRClient();
await connection1.StartAsync();
var createResponse = await connection1.InvokeAsync<Create.Response>("create");
connection1.On<string>("receiveMessage", message => Console.WriteLine($"connection1 receive : {message}"));


var connection2 = await InitSignalRClient();

await connection2.StartAsync();
var joinResponse = await connection2.InvokeAsync<Create.Response>("join", createResponse.ChannelId);
connection2.On<string>("receiveMessage", message => Console.WriteLine($"connection2 receive : {message}"));

await connection2.SendAsync("sendMessage", createResponse.ChannelId, "hello i'm connection2");
await connection1.SendAsync("sendMessage", createResponse.ChannelId, "hello i'm connection1");

Console.ReadLine();