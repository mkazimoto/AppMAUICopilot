using CameraApp.ViewModels;
using CameraApp.Models;
using CameraApp.Services;

namespace CameraApp.Views;

[QueryProperty(nameof(FormId), "formId")]
public partial class FormEditPage : ContentPage
{
    private readonly FormEditViewModel _viewModel;
    private readonly IFormService _formService;

    public string FormId { get; set; } = string.Empty;

    public FormEditPage(FormEditViewModel viewModel, IFormService formService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _formService = formService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        System.Diagnostics.Debug.WriteLine($"[FormEditPage] OnAppearing - FormId: '{FormId}'");
        System.Diagnostics.Debug.WriteLine($"[FormEditPage] IsNullOrEmpty: {string.IsNullOrEmpty(FormId)}, IsNew: {FormId == "new"}");

        if (!string.IsNullOrEmpty(FormId) && FormId != "new")
        {
            // Modo de edição - carregar formulário existente
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] Modo EDIÇÃO - carregando formulário ID: {FormId}");
            await LoadFormForEdit();
        }
        else
        {
            // Modo de criação - configurar para novo formulário
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] Modo CRIAÇÃO - novo formulário");
            _viewModel.SetCreateMode();
        }
    }

    private async Task LoadFormForEdit()
    {
        try
        {
            _viewModel.IsLoading = true;
            
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] LoadFormForEdit - Iniciando carregamento do formulário ID: {FormId}");

            var form = await _formService.GetFormByIdAsync(FormId);
            
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] Formulário carregado: {form != null}");
            
            if (form != null)
            {
                System.Diagnostics.Debug.WriteLine($"[FormEditPage] Configurando modo de edição para: {form.Title}");
                _viewModel.SetEditMode(form);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[FormEditPage] ERRO: Formulário não encontrado");
                await DisplayAlert("Erro", "Formulário não encontrado.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] EXCEÇÃO ao carregar formulário: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[FormEditPage] StackTrace: {ex.StackTrace}");
            await DisplayAlert("Erro", $"Erro ao carregar formulário: {ex.Message}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _viewModel.IsLoading = false;
        }
    }
}