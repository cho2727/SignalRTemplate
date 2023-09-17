namespace SignalRClient.SignalR;

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

public class Login
{
    public class Response : BaseResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}

public abstract class BaseResponse
{
    public bool Result { get; set; }
    public Error? Error { get; set; }
}

public class Error
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}