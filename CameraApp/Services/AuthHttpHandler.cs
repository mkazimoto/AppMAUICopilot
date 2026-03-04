using System.Text.Json;
using CameraApp.Config;
using CameraApp.Models;
using CameraApp.Services;

namespace CameraApp.Services
{
    /// <summary>
    /// Intercepts outgoing HTTP requests to attach a bearer token and transparently refresh it on 401 responses.
    /// </summary>
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private bool _isRefreshing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHttpHandler" /> class.
        /// </summary>
        /// <param name="authService">The authentication service used to retrieve and refresh access tokens.</param>
        public AuthHttpHandler(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Sends an HTTP request, attaching the bearer token and retrying once after a transparent token refresh on 401.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The HTTP response message from the server.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // A URL não é de autenticação ?
            if (request.RequestUri != null &&
                !request.RequestUri.AbsoluteUri.Contains(ApiConfig.Endpoints.Auth))
            {
                // Primeiro, garante que o token é válido antes de enviar a requisição
                await SetToken(request);

            }

            // Envia a requisição original
            var response = await base.SendAsync(request, cancellationToken);

            if (request.RequestUri != null &&
                !request.RequestUri.AbsoluteUri.Contains(ApiConfig.Endpoints.Auth))
            {
                ApiError? apiError = null;
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        apiError = JsonSerializer.Deserialize<ApiError>(errorContent, ApiConfig.GetJsonOptions());
                    }
                }

                // Se recebeu 401 e ainda não está renovando token, tenta renovar
                if ((apiError?.Code == "FE005" ||
                     response.StatusCode == System.Net.HttpStatusCode.Unauthorized) && !_isRefreshing)
                {
                    _isRefreshing = true;
                    try
                    {
                        var refreshToken = await SecureStorage.GetAsync("refresh_token");

                        if (refreshToken != null)
                        {
                            // Tenta renovar o token
                            var tokenRenewed = await _authService.RefreshTokenAsync(refreshToken.ToString());

                            if (tokenRenewed != null)
                            {
                                await SetToken(request);

                                // Reenvia a requisição com o token renovado
                                response = await base.SendAsync(request, cancellationToken);
                            }
                            else
                            {
                                // Se não conseguiu renovar, força logout
                                await _authService.LogoutAsync();
                            }
                        }
                    }
                    finally
                    {
                        _isRefreshing = false;
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Attaches the current bearer access token to the Authorization header of the request.
        /// </summary>
        /// <param name="request">The HTTP request message to attach the token to.</param>
        private async Task SetToken(HttpRequestMessage request)
        {
            // A URL não é de autenticação ?
            if (request.RequestUri != null &&
                !request.RequestUri.AbsoluteUri.Contains(ApiConfig.Endpoints.Auth))
            {
                var accessToken = await SecureStorage.GetAsync("access_token");

                // Atualiza o header de autorização da requisição
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

        }


    }
}