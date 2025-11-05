using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class PosturePage : ContentPage
{
    public PosturePage(PosturePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}