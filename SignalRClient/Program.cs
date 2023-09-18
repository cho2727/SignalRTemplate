// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using SignalRClient.Extensions;
using SignalRClient.SignalR;
Console.WriteLine("Hello, World!");

async Task<string> GetLoginToken()
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

    return token;
}

async Task<HubConnection> InitSignalRClient()
{
    // string token = await GetLoginToken();
    // amazone cognito authority
    string token = "eyJraWQiOiJwdjFKNXV1eFFXY0pySlBNMkQ1XC9mdFRGXC9WVitpc1hldHVkWG5BTEtcL0lZPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI5MzNlMDg3ZC0yMTY5LTRmYzEtYWIzMy04MDI1NmNjNzEwZmEiLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtbm9ydGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtbm9ydGhlYXN0LTJfSG9ndGNOd1VvIiwiY2xpZW50X2lkIjoiNGUxNjA3ajQ5cjdwaXIwM2FuczhvMzBob3IiLCJvcmlnaW5fanRpIjoiODY5Yjc1OWMtNGZjMi00ZmYwLWEzYTEtNzBhOTcyMDhlNWM1IiwiZXZlbnRfaWQiOiI1NWM3YmJhYS1kOTIwLTQ5NDktYTYxYi05ZDQxMWQzY2YyYzUiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIiwiYXV0aF90aW1lIjoxNjk1MDI2NDkwLCJleHAiOjE2OTUwMzAwOTAsImlhdCI6MTY5NTAyNjQ5MCwianRpIjoiYjhlMWQxOTMtNGQyNy00MGVkLTk4ZDItOThjZjIwYmEyYmY1IiwidXNlcm5hbWUiOiI5MzNlMDg3ZC0yMTY5LTRmYzEtYWIzMy04MDI1NmNjNzEwZmEifQ.oWnU-RrTJjlWu1nNaNm2Rnb-T6Du45WfNK28h4XGll5VH0N32Vaq00RpCJYjTF8o1KIvpRHkckMxHEMz-nf5SzY93cr4ZpKN1tTSG9leUKpMVmy0hAoUBaVJg3UJMmKgO2YsDWOqT4oYzEbWxSWIhMDdLKSWSnSaBnxSBt-9aFVCpxhNOMyat5qPyWP0d6tMFHOWNrw6ST1a1wGx6oN0Bnd-clVIXD4rtsDjc34YXuzfB7q7BbFsC6oSLu575xJFg75jUBGRTlFGC2lqwjjQGSVxXrL_wVD7b-iM107EStd-8_1a9YnAu7ReIzGkaZBzdaXf2bcu-QLF7iSejyjLew";

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