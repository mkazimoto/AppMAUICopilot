using CameraApp.Models;

namespace CameraApp.Services
{
    /// <summary>
    /// Defines authentication operations for logging in, refreshing tokens, and managing session state.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates the user with the provided credentials and returns an access token.
        /// </summary>
        /// <param name="username">The user's login name.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="serviceAlias">An optional service alias used to scope authentication.</param>
        /// <returns>The authentication token on success; <see langword="null" /> if authentication fails.</returns>
        Task<AuthToken?> LoginAsync(string username, string password, string? serviceAlias = null);

        /// <summary>
        /// Exchanges a refresh token for a new access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to exchange.</param>
        /// <returns>A new authentication token on success; <see langword="null" /> if the exchange fails.</returns>
        Task<AuthToken?> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Clears the current session and removes stored credentials.
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Attempts to restore a previously saved authentication session from secure storage.
        /// </summary>
        /// <returns><see langword="true" /> if a valid session was restored; otherwise, <see langword="false" />.</returns>
        Task<bool> TryRestoreTokenAsync();

        /// <summary>
        /// Gets a value that indicates whether the current session is authenticated and the token has not expired.
        /// </summary>
        /// <value><see langword="true" /> if the user is authenticated with a valid token; otherwise, <see langword="false" />.</value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the current bearer access token, or <see langword="null" /> if not authenticated.
        /// </summary>
        /// <value>The access token string, or <see langword="null" /> if no active session exists.</value>
        string? CurrentToken { get; }

        /// <summary>
        /// Occurs when the authentication state changes.
        /// </summary>
        /// <remarks>The event argument is <see langword="true" /> when the user becomes authenticated, and <see langword="false" /> when the session ends.</remarks>
        event EventHandler<bool> AuthenticationChanged;
    }
}