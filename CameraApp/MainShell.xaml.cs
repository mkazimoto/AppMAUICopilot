using CameraApp.Services;

namespace CameraApp;

public partial class MainShell : Shell
{
    private readonly IAuthService _authService;

    public MainShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await _authService.LogoutAsync();
        
        // Navegar de volta para a tela de login
        Application.Current!.Windows[0].Page = new AppShell();
    }
}