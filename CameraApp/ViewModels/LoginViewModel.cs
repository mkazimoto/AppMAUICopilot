using CameraApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel; // Para MainThread

namespace CameraApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginViewModel> _logger;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string serviceAlias = string.Empty;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isAuthenticated = false;

        [ObservableProperty]
        private string loginButtonText = "Entrar";

        public LoginViewModel(IAuthService authService, ILogger<LoginViewModel> logger)
        {
            _authService = authService;
            _logger = logger;
            
            // Escutar mudanças na autenticação
            _authService.AuthenticationChanged += OnAuthenticationChanged;
            
            // Verificar se já está autenticado
            IsAuthenticated = _authService.IsAuthenticated;
            UpdateButtonText();
            
            // Tentar restaurar sessão automaticamente
            _ = Task.Run(async () => await TryRestoreSessionAsync());
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validações básicas
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Por favor, informe o nome de usuário.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Por favor, informe a senha.";
                    return;
                }

                var token = await _authService.LoginAsync(Username, Password, 
                    string.IsNullOrWhiteSpace(ServiceAlias) ? null : ServiceAlias);

                if (token != null)
                {
                    _logger.LogInformation("Login realizado com sucesso");
                    
                    // Navegar para o app principal
                    Application.Current!.Windows[0].Page = new MainShell(_authService);
                }
                else
                {
                    ErrorMessage = "Credenciais inválidas. Verifique o usuário e senha.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erro ao fazer login. Tente novamente.";
                _logger.LogError(ex, "Erro durante o login");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (IsLoading) return;

            // Exibir confirmação antes de prosseguir
            bool confirm = await MainThread.InvokeOnMainThreadAsync(async () =>
                await (Shell.Current?.DisplayAlert("Confirmação", "Deseja realmente sair?", "Sair", "Cancelar") ?? Task.FromResult(false)));
            if (!confirm)
            {
                return; // Usuário cancelou
            }

            try
            {
                IsLoading = true;
                await _authService.LogoutAsync();
                
                // Limpar campos
                Username = string.Empty;
                Password = string.Empty;
                ServiceAlias = string.Empty;
                ErrorMessage = string.Empty;
                
                _logger.LogInformation("Logout realizado com sucesso");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erro ao fazer logout.";
                _logger.LogError(ex, "Erro durante o logout");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ClearCredentials()
        {
            Username = string.Empty;
            Password = string.Empty;
            ServiceAlias = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void OnAuthenticationChanged(object? sender, bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
            UpdateButtonText();
            
            if (isAuthenticated)
            {
                ErrorMessage = "Login realizado com sucesso!";
            }
        }

        private void UpdateButtonText()
        {
            LoginButtonText = IsAuthenticated ? "Sair" : "Entrar";
        }

        private async Task TryRestoreSessionAsync()
        {
            try
            {
                if (_authService is AuthService authService)
                {
                    var tokenRestored = await authService.TryRestoreTokenAsync();
                    
                    if (tokenRestored && _authService.IsAuthenticated)
                    {
                        // Se conseguiu restaurar a sessão, navegar para o app principal
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current!.Windows[0].Page = new MainShell(_authService);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar restaurar sessão");
            }
        }

        public bool CanLogin => !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        public bool CanLogout => !IsLoading && IsAuthenticated;
        public bool ShowLoginForm => !IsAuthenticated;
        public bool ShowLogoutSection => IsAuthenticated;

        partial void OnUsernameChanged(string value) => OnPropertyChanged(nameof(CanLogin));
        partial void OnPasswordChanged(string value) => OnPropertyChanged(nameof(CanLogin));
        partial void OnIsLoadingChanged(bool value)
        {
            OnPropertyChanged(nameof(CanLogin));
            OnPropertyChanged(nameof(CanLogout));
        }
        partial void OnIsAuthenticatedChanged(bool value)
        {
            OnPropertyChanged(nameof(ShowLoginForm));
            OnPropertyChanged(nameof(ShowLogoutSection));
            OnPropertyChanged(nameof(CanLogout));
        }
    }
}