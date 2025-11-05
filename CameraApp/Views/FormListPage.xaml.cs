using CameraApp.ViewModels;

namespace CameraApp.Views;

public partial class FormListPage : ContentPage
{
    public FormListPage(FormListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is FormListViewModel viewModel)
        {
            await viewModel.LoadFormsAsync();
        }
    }
}