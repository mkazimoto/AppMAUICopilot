using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Models;
using CameraApp.Services;
using CameraApp.Exceptions;
using CameraApp.Views;

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
    private DateTime startDate = DateTime.Today.AddDays(-30);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string searchTitle = string.Empty;

    [ObservableProperty]
    private string createdBy = string.Empty;

    [ObservableProperty]
    private bool filterSequentialScript;

    [ObservableProperty]
    private int? minScore;

    [ObservableProperty]
    private int? maxScore;

    [ObservableProperty]
    private int orderByIndex;

    [ObservableProperty]
    private bool orderAscending = true;

    [ObservableProperty]
    private FormFilter currentFilter = FormFilter.Default();

    [ObservableProperty]
    private bool hasActiveFilters;

    [ObservableProperty]
    private int activeFiltersCount;

    // Listas para os Pickers
    [ObservableProperty]
    private ObservableCollection<CategoryItem> categories = new();

    [ObservableProperty]
    private CategoryItem? selectedCategoryItem;

    [ObservableProperty]
    private ObservableCollection<StatusItem> statusItems = new();

    [ObservableProperty]
    private StatusItem? selectedStatusItem;

    public FormListViewModel(IFormService formService, IAuthService authService)
    {
        _formService = formService;
        _authService = authService;

          // Categorias de exemplo - em produção, viria da API
        Categories.Add(new CategoryItem { Id = 0, Name = "Todas as Categorias" });
        Categories.Add(new CategoryItem { Id = 1, Name = "Tarefa" });
        Categories.Add(new CategoryItem { Id = 2, Name = "Medição" });
        Categories.Add(new CategoryItem { Id = 3, Name = "Movimento" });
        Categories.Add(new CategoryItem { Id = 4, Name = "Projeto" });

        // Status de exemplo - em produção, viria da API
        StatusItems.Add(new StatusItem { Id = -1, Name = "Todos os Status" });
        StatusItems.Add(new StatusItem { Id = 0, Name = "Rascunho" });
        StatusItems.Add(new StatusItem { Id = 1, Name = "Publicado" });
        StatusItems.Add(new StatusItem { Id = 2, Name = "Inativo" });
    }

    [RelayCommand]
    public async Task LoadFormsAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            await LoadFormsInternalAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        try
        {
            IsLoading = true;
            await LoadFormsInternalAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadFormsInternalAsync()
    {
        try
        {
            CurrentPage = 1;
            
            // Verificar autenticação antes de fazer a chamada
            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }
            
            var response = await _formService.GetFormsAsync(CurrentFilter);
            
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
            
            // Usa o filtro atual mas com a nova página
            var filter = CurrentFilter.Clone();
            filter.Page = CurrentPage;
            var response = await _formService.GetFormsAsync(filter);
            
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
    public async Task ClearFiltersAsync()
    {
        SelectedCategoryItem = null;
        SelectedStatusItem = null;
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        SearchText = string.Empty;
        SearchTitle = string.Empty;
        CreatedBy = string.Empty;
        FilterSequentialScript = false;
        MinScore = null;
        MaxScore = null;
        OrderByIndex = 0;
        OrderAscending = true;
        
        CurrentFilter = FormFilter.Default();
        UpdateFilterIndicators();
        
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
    public async Task SearchByTitleAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;

            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }

            // Cria filtro com título
            CurrentFilter = new FormFilter
            {
                Title = SearchTitle,
                Page = CurrentPage,
                PageSize = 10
            };

            var response = await _formService.GetFormsAsync(CurrentFilter);

            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }

            HasNextPage = response.HasNext;
            UpdateFilterIndicators();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("buscar por título", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task OpenAdvancedFiltersAsync()
    {
        await Shell.Current.GoToAsync(nameof(AdvancedFiltersPage));
    }

    [RelayCommand]
    public async Task ApplyAdvancedFiltersAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            CurrentPage = 1;

            if (!await EnsureUserIsAuthenticatedAsync())
            {
                return;
            }

            // Constrói o filtro com todos os critérios
            CurrentFilter = new FormFilter
            {
                Title = SearchTitle,
                CategoryId = SelectedCategoryItem?.Id > 0 ? SelectedCategoryItem?.Id : null,
                StatusFormId = SelectedStatusItem?.Id >= 0 ? SelectedStatusItem?.Id : null,
                StartDate = StartDate != default ? StartDate : null,
                EndDate = EndDate != default ? EndDate : null,
                CreatedBy = !string.IsNullOrEmpty(CreatedBy) ? CreatedBy : null,
                SequentialScript = FilterSequentialScript ? true : null,
                MinScore = MinScore,
                MaxScore = MaxScore,
                OrderBy = GetOrderByField(),
                OrderAscending = OrderAscending,
                Page = CurrentPage,
                PageSize = 10
            };

            var response = await _formService.GetFormsAsync(CurrentFilter);

            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }

            HasNextPage = response.HasNext;
            UpdateFilterIndicators();

            // Volta para a página de lista
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("aplicar filtros", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ClearAdvancedFilters()
    {
        SelectedCategoryItem = null;
        SelectedStatusItem = null;
        SelectedCategoryItem = null;
        SelectedStatusItem = null;
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        CreatedBy = string.Empty;
        FilterSequentialScript = false;
        MinScore = null;
        MaxScore = null;
        OrderByIndex = 0;
        OrderAscending = true;
    }

    private string? GetOrderByField()
    {
        return OrderByIndex switch
        {
            0 => "title",
            1 => "recCreatedOn",
            2 => "recModifiedOn",
            3 => "totalScore",
            4 => "categoryId",
            _ => null
        };
    }

    private void UpdateFilterIndicators()
    {
        HasActiveFilters = CurrentFilter.HasFilters || !string.IsNullOrEmpty(SearchTitle);
        
        ActiveFiltersCount = 0;
        if (!string.IsNullOrEmpty(SearchTitle)) ActiveFiltersCount++;
        if (CurrentFilter.CategoryId.HasValue) ActiveFiltersCount++;
        if (CurrentFilter.StatusFormId.HasValue) ActiveFiltersCount++;
        if (CurrentFilter.StartDate.HasValue || CurrentFilter.EndDate.HasValue) ActiveFiltersCount++;
        if (!string.IsNullOrEmpty(CurrentFilter.CreatedBy)) ActiveFiltersCount++;
        if (CurrentFilter.SequentialScript.HasValue) ActiveFiltersCount++;
        if (CurrentFilter.MinScore.HasValue || CurrentFilter.MaxScore.HasValue) ActiveFiltersCount++;
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
            
            // Trocar a página raiz para AppShell (que contém LoginPage)
            var window = Application.Current?.Windows?.FirstOrDefault();
            if (window != null)
            {
                window.Page = new AppShell();
            }
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
        return true;
    }
}