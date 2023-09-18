using System.Text.Json.Serialization;

namespace SignalRTemplate.Shared.Interface
{
    public class OpenIdConfiguration
    {
        [JsonPropertyName("authorization_endpoint")]
        public string? AuthorizationEndpoint { get; set; }
        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public string[]? IdTokenSigningAlgValuesSupported { get; set; }
        [JsonPropertyName("issuer")]
        public string? Issuer { get; set; }
        [JsonPropertyName("jwks_uri")]
        public string? JwksUri { get; set; }
        [JsonPropertyName("response_types_supported")]
        public string[]? ResponseTypesSupported { get; set; }
        [JsonPropertyName("scopes_supported")]
        public string[]? ScopesSupported { get; set; }
        [JsonPropertyName("subject_types_supported")]
        public string[]? SubjectTypesSupported { get; set; }
        [JsonPropertyName("token_endpoint")]
        public string? TokenEndpoint { get; set; }
        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public string[]? TokenEndpointAuthMethodsSupported { get; set; }
        [JsonPropertyName("userinfo_endpoint")]
        public string? UserinfoEndpoint { get; set; }
    }
}
