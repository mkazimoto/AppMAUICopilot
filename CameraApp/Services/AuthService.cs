using CameraApp.Models;
using CameraApp.Config;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace CameraApp.Services
{
  public class AuthService : IAuthService
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private AuthToken? _currentToken;

    public event EventHandler<bool>? AuthenticationChanged;

    public bool IsAuthenticated => _currentToken != null && !_currentToken.IsExpired;
    public string? CurrentToken => _currentToken?.AccessToken;

    public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
    {
      _httpClient = httpClient;
      _logger = logger;
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
            await SecureStorage.SetAsync("access_token", tokenResponse.AccessToken);
            await SecureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
            await SecureStorage.SetAsync("token_expires_at", tokenResponse.ExpiresAt.ToString());

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
            await SecureStorage.SetAsync("access_token", tokenResponse.AccessToken);
            await SecureStorage.SetAsync("refresh_token", tokenResponse.RefreshToken);
            await SecureStorage.SetAsync("token_expires_at", tokenResponse.ExpiresAt.ToString());

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
        SecureStorage.RemoveAll();

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
        var accessToken = await SecureStorage.GetAsync("access_token");
        var refreshToken = await SecureStorage.GetAsync("refresh_token");
        var expiresAtString = await SecureStorage.GetAsync("token_expires_at");

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

    public async Task<bool> EnsureValidTokenAsync()
    {
      try
      {
        // Se não há token, não pode renovar automaticamente
        if (_currentToken == null)
        {
          _logger.LogWarning("Nenhum token disponível para validação");
          return false;
        }

        // Se o token ainda é válido e não está expirando em breve, retorna true
        if (!_currentToken.IsExpired)
        {
          return true;
        }
        var refreshToken = _currentToken.RefreshToken;

        // Token expirado ou expirando em breve, tentar renovar
        if (_currentToken.IsExpired)
        {
          var newToken = await RefreshTokenAsync(refreshToken);
          if (newToken != null)
          {
            _logger.LogInformation("Token renovado automaticamente com sucesso");
            return true;
          }

          _logger.LogInformation("Token expirado, tentando renovar automaticamente");
        }

        _logger.LogError("Falha na renovação automática do token");
        await LogoutAsync(); // Força logout se renovação falhou
        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro durante validação/renovação automática do token");
        await LogoutAsync(); // Força logout em caso de erro
        return false;
      }
    }
  }
}