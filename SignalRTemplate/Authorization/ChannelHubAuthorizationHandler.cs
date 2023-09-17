using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace SignalRTemplate.Authorization;


public class ChannelHubAuthorizationRequirement : IAuthorizationRequirement
{

}


public class ChannelHubAuthorizationHandler : AuthorizationHandler<ChannelHubAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChannelHubAuthorizationHandler> _logger;

    public ChannelHubAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<ChannelHubAuthorizationHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ChannelHubAuthorizationRequirement requirement)
    {
        var request = _httpContextAccessor.HttpContext.Request;
        string jwtToken = string.Empty;
        var token = request?.Headers["Authorization"];
        AuthenticationHeaderValue.TryParse(token, out var tokenValue);
        jwtToken = tokenValue?.Parameter ?? "";

        jwtToken = jwtToken == "" ? request?.Query["access_token"].FirstOrDefault() ?? string.Empty : jwtToken;

        if (await ValidateTokenAsync(jwtToken))
        {
            _logger.LogDebug("Signalr authorization succeed");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogDebug("Signalr authorization failed");
            context.Fail();
        }
    }

    private async ValueTask<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var url = _configuration.GetSection("HostService").GetValue<string>("Identity");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return true;
            }

            return false;

        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            return false;
        }
    }
}
