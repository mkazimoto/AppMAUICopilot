using CameraApp.Config;
using CameraApp.Services;

namespace CameraApp.Services
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private bool _isRefreshing = false;

        public AuthHttpHandler(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Intercepta requisições HTTP para adicionar o token de autenticação.
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Primeiro, garante que o token é válido antes de enviar a requisição
            await SetToken(request);

            // Envia a requisição original
            var response = await base.SendAsync(request, cancellationToken);

            // Se recebeu 401 e ainda não está renovando token, tenta renovar
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !_isRefreshing)
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

            return response;
        }

        /// <summary>
        /// Seta a token de acesso na requisição HTTP
        /// </summary>
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