using System.Text.Json.Serialization;

namespace CameraApp.Models
{
    public class AuthToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; } = 300; // 5 minutos por padrão

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.Now >= ExpiresAt;
                
        public TimeSpan TimeUntilExpiration => ExpiresAt - DateTime.Now;
    }

    public class LoginRequest
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "password";

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("servicealias")]
        public string? ServiceAlias { get; set; }
    }

    public class RefreshTokenRequest
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "refresh_token";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}