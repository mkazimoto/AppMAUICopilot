using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class AdvancedFiltersPage : ContentPage
{
    public AdvancedFiltersPage(FormListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
