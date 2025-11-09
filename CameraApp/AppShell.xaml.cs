using CameraApp.Views;

namespace CameraApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Registrar rotas para navegação
		Routing.RegisterRoute("FormEditPage", typeof(FormEditPage));
		Routing.RegisterRoute(nameof(AdvancedFiltersPage), typeof(AdvancedFiltersPage));
	}
}
