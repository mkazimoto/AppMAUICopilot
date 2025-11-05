using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Models;
using CameraApp.Services;
using CameraApp.Exceptions;

namespace CameraApp.ViewModels;

public partial class FormListViewModel : ObservableObject
{
    private readonly IFormService _formService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<Form> forms = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool hasNextPage;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int selectedCategoryId;

    [ObservableProperty]
    private int selectedStatusId;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private string searchText = string.Empty;

    public FormListViewModel(IFormService formService, IAuthService authService)
    {
        _formService = formService;
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoadFormsAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }
            
            var response = await _formService.GetFormsAsync(CurrentPage, 10);
            
            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
            
            HasNextPage = response.HasNext;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("carregar formulários", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadMoreFormsAsync()
    {
        if (IsLoading || !HasNextPage) return;

        try
        {
            IsLoading = true;
            CurrentPage++;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                CurrentPage--; // Revert page increment
                return;
            }
            
            var response = await _formService.GetFormsAsync(CurrentPage, 10);
            
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
            
            HasNextPage = response.HasNext;
        }
        catch (Exception ex)
        {
            CurrentPage--; // Revert page increment on error
            await ShowErrorAsync("carregar mais formulários", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task FilterByCategoryAsync()
    {
        if (IsLoading || SelectedCategoryId <= 0) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }
            
            var response = await _formService.GetFormsByCategoryAsync(SelectedCategoryId, CurrentPage, 10);
            
            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
            
            HasNextPage = response.HasNext;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("filtrar por categoria", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task FilterByStatusAsync()
    {
        if (IsLoading || SelectedStatusId <= 0) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }
            
            var response = await _formService.GetFormsByStatusAsync(SelectedStatusId, CurrentPage, 10);
            
            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
            
            HasNextPage = response.HasNext;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("filtrar por status", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task FilterByDateRangeAsync()
    {
        if (IsLoading || StartDate > EndDate) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }
            
            var response = await _formService.GetFormsByDateRangeAsync(StartDate, EndDate, CurrentPage, 10);
            
            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
            
            HasNextPage = response.HasNext;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("filtrar por período", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ClearFiltersAsync()
    {
        SelectedCategoryId = 0;
        SelectedStatusId = 0;
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        SearchText = string.Empty;
        
        await LoadFormsAsync();
    }

    [RelayCommand]
    public async Task CreateNewFormAsync()
    {
        await Shell.Current.GoToAsync($"FormEditPage?formId=new");
    }

    [RelayCommand]
    public async Task EditFormAsync(Form form)
    {
        System.Diagnostics.Debug.WriteLine($"[FormListViewModel] EditFormAsync chamado");
        System.Diagnostics.Debug.WriteLine($"[FormListViewModel] Form: {form?.Title}, ID: {form?.Id}");
        
        if (form?.Id != null)
        {
            System.Diagnostics.Debug.WriteLine($"[FormListViewModel] Navegando para FormEditPage com formId={form.Id}");
            await Shell.Current.GoToAsync($"FormEditPage?formId={form.Id}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[FormListViewModel] ERRO: Form ou Form.Id é null");
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadFormsAsync();
    }

    private async Task ShowErrorAsync(string operation, Exception ex)
    {
        if (Application.Current?.Windows?.FirstOrDefault()?.Page is not Page currentPage)
            return;

        if (ex is UnauthorizedAccessException)
        {
            // Erro de autenticação - redirecionar para login
            await currentPage.DisplayAlert("Sessão Expirada", 
                "Sua sessão expirou. Você será redirecionado para fazer login novamente.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else if (ex is ApiException apiEx)
        {
            var title = $"Erro da API ({apiEx.ApiError.Code})";
            var message = apiEx.ApiError.GetDisplayMessage();
            
            // Para o erro FE018 específico, adiciona uma dica
            if (apiEx.ApiError.Code == "FE018")
            {
                message += "\n\nDica: Verifique se os filtros estão corretos ou tente limpar os filtros.";
            }
            
            await currentPage.DisplayAlert(title, message, "OK");
        }
        else
        {
            await currentPage.DisplayAlert("Erro", 
                $"Erro inesperado ao {operation}: {ex.Message}", "OK");
        }
    }

    private async Task<bool> EnsureUserIsAuthenticatedAsync()
    {
        try
        {
            // Verifica se o usuário está autenticado e se o token é válido
            if (!_authService.IsAuthenticated)
            {
                if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page currentPage)
                {
                    await currentPage.DisplayAlert("Erro", "Você precisa fazer login para acessar os formulários.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                return false;
            }

            // Tenta garantir que o token é válido (renovação automática se necessário)
            var tokenValid = await _authService.EnsureValidTokenAsync();
            if (!tokenValid)
            {
                if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page currentPage)
                {
                    await currentPage.DisplayAlert("Sessão Expirada", "Sua sessão expirou. Faça login novamente.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page currentPage)
            {
                await currentPage.DisplayAlert("Erro", $"Erro de autenticação: {ex.Message}", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            return false;
        }
    }
}