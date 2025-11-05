using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class CameraPage : ContentPage
{
    public CameraPage(CameraPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}