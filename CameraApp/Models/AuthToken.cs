using System.Text.Json.Serialization;

namespace CameraApp.Models
{
    /// <summary>
    /// Represents an OAuth 2.0 authentication token returned by the API.
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        /// Gets or sets the bearer access token used in API requests.
        /// </summary>
        /// <value>The JWT access token string. The default is an empty string.</value>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token type, typically <c>Bearer</c>.
        /// </summary>
        /// <value>The token type identifier. The default is an empty string.</value>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of seconds until the access token expires.
        /// </summary>
        /// <value>The lifetime of the access token in seconds.</value>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token used to obtain a new access token.
        /// </summary>
        /// <value>The refresh token string. The default is an empty string.</value>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the absolute date and time at which the access token expires.
        /// </summary>
        /// <value>The UTC expiration date and time of the token.</value>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the access token has expired.
        /// </summary>
        /// <value><see langword="true" /> if the current time is past <see cref="ExpiresAt" />; otherwise, <see langword="false" />.</value>
        public bool IsExpired => DateTime.Now >= ExpiresAt;

        /// <summary>
        /// Gets the remaining time before the access token expires.
        /// </summary>
        /// <value>A <see cref="TimeSpan" /> representing the time until expiration; negative if already expired.</value>
        public TimeSpan TimeUntilExpiration => ExpiresAt - DateTime.Now;
    }

    /// <summary>
    /// Represents an OAuth 2.0 resource owner password credentials grant request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the OAuth 2.0 grant type.
        /// </summary>
        /// <value>The grant type identifier. The default is <c>"password"</c>.</value>
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "password";

        /// <summary>
        /// Gets or sets the user's login name.
        /// </summary>
        /// <value>The username credential. The default is an empty string.</value>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>The password credential. The default is an empty string.</value>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional service alias used to scope authentication.
        /// </summary>
        /// <value>The service alias, or <see langword="null" /> if not required.</value>
        [JsonPropertyName("servicealias")]
        public string? ServiceAlias { get; set; }
    }

    /// <summary>
    /// Represents an OAuth 2.0 refresh token grant request.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Gets or sets the OAuth 2.0 grant type.
        /// </summary>
        /// <value>The grant type identifier. The default is <c>"refresh_token"</c>.</value>
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "refresh_token";

        /// <summary>
        /// Gets or sets the refresh token to exchange for a new access token.
        /// </summary>
        /// <value>The refresh token string. The default is an empty string.</value>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}