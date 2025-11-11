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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Primeiro, garante que o token é válido antes de enviar a requisição
            await EnsureValidTokenAsync(request);

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
                            // Token renovado, atualiza o header da requisição original e reenvia                            
                            var newToken = tokenRenewed.AccessToken;
                            if (!string.IsNullOrEmpty(newToken))
                            {
                                request.Headers.Authorization =
                                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);

                                // Reenvia a requisição com o token renovado
                                response = await base.SendAsync(request, cancellationToken);
                            }
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

        private async Task EnsureValidTokenAsync(HttpRequestMessage request)
        {
            if (!_isRefreshing)
            {
                var accessToken = await SecureStorage.GetAsync("access_token");

                // Atualiza o header de autorização da requisição
                if (accessToken != null)
                {
                    var token = accessToken;
                    if (!string.IsNullOrEmpty(token))
                    {
                        request.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                }
            }
        }
    }
}