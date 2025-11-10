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
    // Exibir confirmação antes de prosseguir
    bool confirm = await MainThread.InvokeOnMainThreadAsync(async () =>
        await (Shell.Current?.DisplayAlert("Confirmação", "Deseja realmente sair?", "Sair", "Cancelar") ?? Task.FromResult(false)));

    if (!confirm)
      return;

    await _authService.LogoutAsync();

    // Trocar para o AppShell (tela de login)
    await MainThread.InvokeOnMainThreadAsync(() =>
    {
      if (Application.Current?.Windows.Count > 0)
      {
        Application.Current.Windows[0].Page = new AppShell();
      }
    });
  }
}