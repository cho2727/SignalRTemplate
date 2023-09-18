using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SignalRTemplate.Extensions;
using SignalRTemplate.Shared.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
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
    private readonly HttpClient _httpClient;

    public ChannelHubAuthorizationHandler(IHttpContextAccessor httpContextAccessor, 
                                        IConfiguration configuration, 
                                        ILogger<ChannelHubAuthorizationHandler> logger,
                                        IHttpClientFactory httpClientFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ChannelHubAuthorizationRequirement requirement)
    {
        var request = _httpContextAccessor.HttpContext.Request;
        string jwtToken = string.Empty;
        var token = request?.Headers["Authorization"];
        AuthenticationHeaderValue.TryParse(token, out var tokenValue);
        jwtToken = tokenValue?.Parameter ?? "";

        jwtToken = jwtToken == "" ? request?.Query["access_token"].FirstOrDefault() ?? string.Empty : jwtToken;

        if (ValidateToken(jwtToken))
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
    private bool ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                },
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                {
                    // openid configuration 정보 가져오기
                    var url = $"{securityToken.Issuer}/.well-known/openid-configuration";
                    var configData = GetData(url);
                    var openIdConfig = configData.DeserializeFromJson<OpenIdConfiguration>();

                    // JwksUri 로 부터 jsonwebkey list 가져오기
                    var jwksData = GetData(openIdConfig?.JwksUri);
                    var jsonWebKeySet = new JsonWebKeySet(jwksData);

                    return jsonWebKeySet.Keys;
                },
            };

            var principal = handler.ValidateToken(token, validationParameters, out var jwtToken);

            return true;

        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            return false;
        }
    }

    private string GetData(string? url)
    {
        if (url == null) return string.Empty;


        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = _httpClient.Send(request);
        response.EnsureSuccessStatusCode();
        using var stream = response.Content.ReadAsStream();
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}
