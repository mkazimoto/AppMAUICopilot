using CameraApp.Services;

namespace CameraApp;

public partial class App : Application
{
	private readonly IAuthService _authService;

	public App(IAuthService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override async void OnStart()
	{
		base.OnStart();
		
		// Tentar restaurar token do SecureStorage
		var tokenRestored = await _authService.TryRestoreTokenAsync();
		
		if (!tokenRestored)
		{
			// Se não conseguiu restaurar o token, ir para a tela de login
			await Shell.Current.GoToAsync("//LoginPage");
		}
	}
}