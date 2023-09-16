namespace SignalRTemplate.SignalR;

public class Create
{
    public class Response
    {
        public string ChannelId { get; set; } = string.Empty;
        public string JoinId { get; set; } = string.Empty;
    }
}

public class Join
{
    public class Response
    {
        public string JoinId { get; set; } = string.Empty;
    }
}