using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CameraApp.Models;
using CameraApp.Services;

namespace CameraApp.ViewModels;

/// <summary>
/// Provides properties and commands for creating and editing a <see cref="Form" /> entity.
/// </summary>
public partial class FormEditViewModel : ObservableObject
{
    private readonly IFormService _formService;

    [ObservableProperty]
    private string formId = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private CategoryItem categoryItem;

    [ObservableProperty]
    private StatusItem statusItem;

    [ObservableProperty]
    private bool sequentialScript;

    [ObservableProperty]
    private int totalScore;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string pageTitle = "Novo Formulário";

    [ObservableProperty]
    private bool canDelete;

    // Listas para os Pickers
    [ObservableProperty]
    private ObservableCollection<CategoryItem> categories = new();

    [ObservableProperty]
    private ObservableCollection<StatusItem> statusItems = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FormEditViewModel" /> class and populates the picker lists.
    /// </summary>
    /// <param name="formService">The form service used to create, update, and delete forms.</param>
    public FormEditViewModel(IFormService formService)
    {
        _formService = formService;
        InitializeLists();
    }

    private void InitializeLists()
    {
        // Categorias de exemplo - em produção, viria da API
        Categories.Add(new CategoryItem { Id = 1, Name = "Tarefa" });
        Categories.Add(new CategoryItem { Id = 2, Name = "Medição" });
        Categories.Add(new CategoryItem { Id = 3, Name = "Movimento" });
        Categories.Add(new CategoryItem { Id = 4, Name = "Projeto" });

        // Status de exemplo - em produção, viria da API
        StatusItems.Add(new StatusItem { Id = 0, Name = "Rascunho" });
        StatusItems.Add(new StatusItem { Id = 1, Name = "Publicado" });
        StatusItems.Add(new StatusItem { Id = 2, Name = "Inativo" });
    }

    /// <summary>
    /// Configures the ViewModel for editing an existing form.
    /// </summary>
    /// <param name="form">The form to edit.</param>
    public void SetEditMode(Form form)
    {
        IsEditMode = true;
        CanDelete = true;
        PageTitle = "Editar Formulário";
        
        FormId = form.Id;
        Title = form.Title;
        CategoryItem = Categories.First(c => c.Id == form.CategoryId);
        StatusItem = StatusItems.First(s => s.Id == form.StatusFormId);
        SequentialScript = form.SequentialScript;
        TotalScore = form.TotalScore;
    }

    /// <summary>
    /// Configures the ViewModel for creating a new form with default values.
    /// </summary>
    public void SetCreateMode()
    {
        IsEditMode = false;
        CanDelete = false;
        PageTitle = "Novo Formulário";
        
        FormId = string.Empty;
        Title = string.Empty;
        CategoryItem = Categories.First(c => c.Id == 1);
        StatusItem = StatusItems.First(s => s.Id == 0);
        SequentialScript = false;
        TotalScore = 0;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsLoading) return;

        if (!ValidateForm())
        {
            await ShowAlertAsync("Erro", "Por favor, preencha todos os campos obrigatórios.");
            return;
        }

        try
        {
            IsLoading = true;

            var form = new Form
            {
                Id = FormId,
                Title = Title,
                CategoryId = CategoryItem.Id,
                StatusFormId = StatusItem.Id,
                SequentialScript = SequentialScript,
                TotalScore = TotalScore,
                RecCreatedBy = "user", // Em produção, pegar do contexto do usuário
                RecModifiedBy = "user"
            };

            Form? result;

            if (IsEditMode)
            {
                result = await _formService.UpdateFormAsync(FormId, form);
                if (result != null)
                {
                    await ShowAlertAsync("Sucesso", "Formulário atualizado com sucesso!");
                }
                else
                {
                    await ShowAlertAsync("Erro", "Erro ao atualizar formulário.");
                    return;
                }
            }
            else
            {
                result = await _formService.CreateFormAsync(form);
                if (result != null)
                {
                    await ShowAlertAsync("Sucesso", "Formulário criado com sucesso!");
                }
                else
                {
                    await ShowAlertAsync("Erro", "Erro ao criar formulário.");
                    return;
                }
            }

            // Voltar para a página anterior
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await ShowAlertAsync("Erro", $"Erro inesperado: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (IsLoading || !IsEditMode) return;

        var currentPage = Application.Current?.Windows?.FirstOrDefault()?.Page;
        if (currentPage == null) return;

        var loc = LocalizationResourceManager.Instance;
        var confirm = await currentPage.DisplayAlertAsync(
            loc["Common_Confirm"].ToString()!,
            loc["FormEdit_DeleteConfirmMessage"].ToString()!,
            loc["Common_Yes"].ToString()!,
            loc["Common_No"].ToString()!);

        if (!confirm) return;

        try
        {
            IsLoading = true;

            var success = await _formService.DeleteFormAsync(FormId);

            if (success)
            {
                await ShowAlertAsync(loc["Common_Success"].ToString()!, loc["FormEdit_DeleteSuccess"].ToString()!);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await ShowAlertAsync(loc["Common_Error"].ToString()!, loc["FormEdit_DeleteError"].ToString()!);
            }
        }
        catch (Exception ex)
        {
            await ShowAlertAsync(loc["Common_Error"].ToString()!, $"Erro inesperado: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidateForm()
    {
        return !string.IsNullOrWhiteSpace(Title) && 
               CategoryItem != null && 
               StatusItem != null;
    }

    private async Task ShowAlertAsync(string title, string message)
    {
        if (Application.Current?.Windows?.FirstOrDefault()?.Page is Page currentPage)
        {
            await currentPage.DisplayAlertAsync(title, message, "OK");
        }
    }


}

/// <summary>
/// Represents a form category item used in a picker control.
/// </summary>
public partial class CategoryItem : ObservableObject
{
    [ObservableProperty]
    private int id;
    [ObservableProperty]
    private string name = string.Empty;
}

/// <summary>
/// Represents a form status item used in a picker control.
/// </summary>
public partial class StatusItem : ObservableObject
{
    [ObservableProperty]
    private int id;
    [ObservableProperty]
    private string name = string.Empty;
}
