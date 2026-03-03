using CameraApp.Models;
using CameraApp.Config;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace CameraApp.Services
{
    /// <summary>
    /// Provides OAuth 2.0 authentication operations including login, token refresh, and session restoration.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly ISecureStorage _secureStorage;
        private AuthToken? _currentToken;

        /// <inheritdoc/>
        public event EventHandler<bool>? AuthenticationChanged;

        /// <inheritdoc/>
        public bool IsAuthenticated => _currentToken != null && !_currentToken.IsExpired;

        /// <inheritdoc/>
        public string? CurrentToken => _currentToken?.AccessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the authentication endpoint.</param>
        /// <param name="logger">The logger used to record authentication events.</param>
        /// <param name="secureStorage">The secure storage used to persist and retrieve tokens.</param>
        public AuthService(HttpClient httpClient, ILogger<AuthService> logger, ISecureStorage secureStorage)
        {
            _httpClient = httpClient;
            _logger = logger;
            _secureStorage = secureStorage;
        }

        public async Task<AuthToken?> LoginAsync(string username, string password, string? serviceAlias = null)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password,
                    ServiceAlias = serviceAlias
                };

                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Auth}", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<AuthToken>();

                    if (tokenResponse != null)
                    {
                        tokenResponse.ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
                        _currentToken = tokenResponse;

                        // Salvar token no armazenamento seguro
                        await _secureStorage.SetAsync("access_token", tokenResponse.AccessToken);
                        await _secureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
                        await _secureStorage.SetAsync("token_expires_at", tokenResponse.ExpiresAt.ToString());

                        AuthenticationChanged?.Invoke(this, true);
                        _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", username);

                        return tokenResponse;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro no login: {StatusCode} - {Content}", response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processo de login");
            }

            return null;
        }

        public async Task<AuthToken?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var refreshRequest = new RefreshTokenRequest
                {
                    RefreshToken = refreshToken
                };

                var response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.Auth}", refreshRequest);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<AuthToken>();

                    if (tokenResponse != null)
                    {
                        tokenResponse.ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
                        _currentToken = tokenResponse;

                        // Atualizar token no armazenamento seguro
                        await _secureStorage.SetAsync("access_token", tokenResponse.AccessToken);
                        await _secureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
                        await _secureStorage.SetAsync("token_expires_at", tokenResponse.ExpiresAt.ToString());

                        AuthenticationChanged?.Invoke(this, true);
                        _logger.LogInformation("Token renovado com sucesso");

                        return tokenResponse;
                    }
                }
                else
                {
                    _logger.LogError("Erro ao renovar token: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a renovação do token");
            }

            return null;
        }

        public Task LogoutAsync()
        {
            try
            {
                // Limpar token da memória
                _currentToken = null;

                // Limpar dados do armazenamento seguro
                _secureStorage.RemoveAll();

                AuthenticationChanged?.Invoke(this, false);
                _logger.LogInformation("Logout realizado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o logout");
            }

            return Task.CompletedTask;
        }

        public async Task<bool> TryRestoreTokenAsync()
        {
            try
            {
                var accessToken = await _secureStorage.GetAsync("access_token");
                var refreshToken = await _secureStorage.GetAsync("refresh_token");
                var expiresAtString = await _secureStorage.GetAsync("token_expires_at");

                if (!string.IsNullOrEmpty(accessToken) &&
                    !string.IsNullOrEmpty(refreshToken) &&
                    DateTime.TryParse(expiresAtString, out var expiresAt))
                {
                    _currentToken = new AuthToken
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = expiresAt,
                        TokenType = "Bearer"
                    };

                    // Se o token expirou, tentar renovar
                    if (_currentToken.IsExpired)
                    {
                        var newToken = await RefreshTokenAsync(refreshToken);
                        return newToken != null;
                    }

                    AuthenticationChanged?.Invoke(this, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao restaurar token");
            }

            return false;
        }

    }
}