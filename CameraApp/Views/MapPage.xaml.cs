using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class MapPage : ContentPage
{
    private readonly MapPageViewModel _viewModel;

    public MapPage(MapPageViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Carrega a localização automaticamente quando a página aparece
        if (_viewModel.GetLocationCommand.CanExecute(null))
        {
            await _viewModel.GetLocationCommand.ExecuteAsync(null);
        }
    }
}