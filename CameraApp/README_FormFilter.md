# FormFilter - Padrão de Filtros Centralizado

## Visão Geral

O `FormFilter` é uma classe que centraliza todos os critérios de filtro, ordenação e paginação para consultas de formulários. Este padrão elimina a duplicação de parâmetros entre métodos e fornece uma API mais flexível e extensível.

## Estrutura da Classe

### Propriedades de Filtro

```csharp
public class FormFilter
{
    // Filtros básicos
    public int? CategoryId { get; set; }
    public int? StatusFormId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Filtros avançados
    public string? Title { get; set; }
    public string? CreatedBy { get; set; }
    public bool? SequentialScript { get; set; }
    public int? MinScore { get; set; }
    public int? MaxScore { get; set; }
    
    // Paginação
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // Ordenação
    public string? OrderBy { get; set; }
    public bool OrderAscending { get; set; } = true;
}
```

## Uso Básico

### 1. Filtro Simples (sem filtros, apenas paginação)

```csharp
// Usando o método simplificado
var forms = await _formService.GetFormsAsync(page: 1, pageSize: 20);

// Ou criando um filtro padrão
var filter = FormFilter.Default(page: 1, pageSize: 20);
var forms = await _formService.GetFormsAsync(filter);
```

### 2. Filtro por Categoria

```csharp
// Método factory
var filter = FormFilter.ByCategory(categoryId: 5, page: 1, pageSize: 10);
var forms = await _formService.GetFormsAsync(filter);

// Ou criando manualmente
var filter = new FormFilter
{
    CategoryId = 5,
    Page = 1,
    PageSize = 10
};
var forms = await _formService.GetFormsAsync(filter);
```

### 3. Filtro por Status

```csharp
var filter = FormFilter.ByStatus(statusId: 2, page: 1, pageSize: 15);
var forms = await _formService.GetFormsAsync(filter);
```

### 4. Filtro por Período

```csharp
var startDate = new DateTime(2024, 1, 1);
var endDate = new DateTime(2024, 12, 31);
var filter = FormFilter.ByDateRange(startDate, endDate, page: 1, pageSize: 20);
var forms = await _formService.GetFormsAsync(filter);
```

## Uso Avançado

### Combinação de Múltiplos Filtros

```csharp
var filter = new FormFilter
{
    CategoryId = 5,
    StatusFormId = 2,
    StartDate = new DateTime(2024, 1, 1),
    EndDate = DateTime.Now,
    SequentialScript = true,
    MinScore = 70,
    Page = 1,
    PageSize = 20,
    OrderBy = "title",
    OrderAscending = true
};

var forms = await _formService.GetFormsAsync(filter);
```

### Busca por Título

```csharp
var filter = new FormFilter
{
    Title = "Inspeção",
    Page = 1,
    PageSize = 10
};

var forms = await _formService.GetFormsAsync(filter);
```

### Filtro por Faixa de Pontuação

```csharp
var filter = new FormFilter
{
    MinScore = 50,
    MaxScore = 90,
    OrderBy = "totalScore",
    OrderAscending = false, // Maior pontuação primeiro
    Page = 1,
    PageSize = 15
};

var forms = await _formService.GetFormsAsync(filter);
```

## Métodos Auxiliares

### HasFilters

Verifica se há algum filtro ativo (além de paginação):

```csharp
var filter = new FormFilter { CategoryId = 5 };
if (filter.HasFilters)
{
    Console.WriteLine("Filtros ativos detectados");
}
```

### BuildODataFilter

Constrói a query string OData com base nos filtros ativos:

```csharp
var filter = new FormFilter
{
    CategoryId = 5,
    StatusFormId = 2
};

var oDataFilter = filter.BuildODataFilter();
// Resultado: "categoryId eq 5 and statusFormId eq 2"
```

### BuildQueryString

Constrói a query string completa incluindo filtros, ordenação e paginação:

```csharp
var filter = new FormFilter
{
    CategoryId = 5,
    Page = 2,
    PageSize = 20,
    OrderBy = "title"
};

var queryString = filter.BuildQueryString();
// Resultado: "$filter=categoryId eq 5&$orderby=title asc&page=2&pagesize=20"
```

### Reset

Reseta todos os filtros mantendo apenas a configuração de paginação:

```csharp
var filter = new FormFilter
{
    CategoryId = 5,
    StatusFormId = 2,
    Page = 3,
    PageSize = 10
};

filter.Reset();
// Agora: CategoryId = null, StatusFormId = null, mas Page = 3, PageSize = 10
```

### Clone

Cria uma cópia independente do filtro:

```csharp
var originalFilter = new FormFilter { CategoryId = 5 };
var clonedFilter = originalFilter.Clone();

clonedFilter.StatusFormId = 2; // Não afeta o original
```

## ViewModels

### Exemplo em ViewModel com CommunityToolkit.Mvvm

```csharp
public partial class FormsViewModel : ObservableObject
{
    private readonly IFormService _formService;

    [ObservableProperty]
    private ObservableCollection<Form> forms = new();

    [ObservableProperty]
    private FormFilter currentFilter = FormFilter.Default();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public FormsViewModel(IFormService formService)
    {
        _formService = formService;
    }

    [RelayCommand]
    private async Task LoadFormsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var response = await _formService.GetFormsAsync(CurrentFilter);
            
            Forms.Clear();
            foreach (var form in response.Items)
            {
                Forms.Add(form);
            }
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task FilterByCategoryAsync(int categoryId)
    {
        CurrentFilter.CategoryId = categoryId;
        CurrentFilter.Page = 1; // Reset para primeira página
        await LoadFormsAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        CurrentFilter.Reset();
        await LoadFormsAsync();
    }

    [RelayCommand]
    private async Task LoadNextPageAsync()
    {
        CurrentFilter.Page++;
        await LoadFormsAsync();
    }
}
```

### Exemplo com Estado de Filtro Persistente

```csharp
public partial class FormsViewModel : ObservableObject
{
    [ObservableProperty]
    private FormFilter searchFilter = new();

    partial void OnSearchFilterChanged(FormFilter value)
    {
        // Recarrega automaticamente quando o filtro mudar
        _ = LoadFormsAsync();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        // Clone o filtro de busca para o filtro atual
        CurrentFilter = SearchFilter.Clone();
        CurrentFilter.Page = 1;
        _ = LoadFormsAsync();
    }
}
```

## Vantagens do Padrão

### 1. Centralização

Todos os critérios de filtro em um único lugar, eliminando duplicação de código.

**Antes:**
```csharp
Task<FormResponse> GetFormsByCategoryAsync(int categoryId, int page, int pageSize);
Task<FormResponse> GetFormsByStatusAsync(int statusId, int page, int pageSize);
Task<FormResponse> GetFormsByDateRangeAsync(DateTime start, DateTime end, int page, int pageSize);
```

**Depois:**
```csharp
Task<FormResponse> GetFormsAsync(FormFilter filter);
```

### 2. Extensibilidade

Adicionar novos filtros não requer mudanças na assinatura dos métodos.

**Para adicionar um novo filtro:**
```csharp
// Apenas adicione a propriedade em FormFilter
public string? Location { get; set; }

// E atualize o BuildODataFilter
if (!string.IsNullOrEmpty(Location))
    filters.Add($"location eq '{Location}'");
```

### 3. Flexibilidade

Combine múltiplos filtros facilmente sem criar métodos específicos.

```csharp
// Busca complexa em uma linha
var filter = new FormFilter 
{ 
    CategoryId = 5, 
    StatusFormId = 2, 
    MinScore = 70,
    SequentialScript = true 
};
```

### 4. Testabilidade

Filtros são objetos testáveis independentemente.

```csharp
[Fact]
public void BuildODataFilter_WithMultipleFilters_ReturnsCorrectString()
{
    var filter = new FormFilter
    {
        CategoryId = 5,
        StatusFormId = 2
    };

    var result = filter.BuildODataFilter();
    
    Assert.Equal("categoryId eq 5 and statusFormId eq 2", result);
}
```

### 5. Reutilização

Filtros podem ser salvos, clonados e reutilizados.

```csharp
// Salvar filtro favorito
var favoriteFilter = new FormFilter { CategoryId = 5, StatusFormId = 2 };

// Reutilizar depois
var filter = favoriteFilter.Clone();
filter.Page = 2;
```

## Padrões de Nomenclatura

### Propriedades de Filtro
- Use tipos nullable (`int?`, `DateTime?`, `string?`) para filtros opcionais
- Use nomes descritivos que refletem a propriedade do modelo sendo filtrada

### Métodos Factory
- Prefixe com `By` para indicar o critério principal: `ByCategory`, `ByStatus`, `ByDateRange`
- Use `Default` para filtros vazios

### Query Building
- `BuildODataFilter()`: Retorna apenas a parte do filtro OData
- `BuildQueryString()`: Retorna a query string completa incluindo paginação

## Boas Práticas

1. **Sempre valide a entrada do usuário antes de aplicar filtros**
   ```csharp
   if (categoryId <= 0)
   {
       ErrorMessage = "Categoria inválida";
       return;
   }
   CurrentFilter.CategoryId = categoryId;
   ```

2. **Reset para página 1 ao mudar filtros**
   ```csharp
   CurrentFilter.CategoryId = newCategory;
   CurrentFilter.Page = 1; // Evita páginas vazias
   ```

3. **Use Clone para filtros temporários**
   ```csharp
   var tempFilter = CurrentFilter.Clone();
   tempFilter.StatusFormId = 2;
   var previewResults = await _formService.GetFormsAsync(tempFilter);
   // CurrentFilter permanece inalterado
   ```

4. **Forneça feedback visual sobre filtros ativos**
   ```csharp
   public string FilterSummary => 
       CurrentFilter.HasFilters 
           ? $"{GetActiveFilterCount()} filtros ativos" 
           : "Sem filtros";
   ```

## Integração com UI

### XAML Binding

```xml
<Entry Text="{Binding CurrentFilter.Title}" 
       Placeholder="Buscar por título" />

<DatePicker Date="{Binding CurrentFilter.StartDate}" />

<Button Text="Aplicar Filtros" 
        Command="{Binding LoadFormsCommand}" />

<Button Text="Limpar Filtros" 
        Command="{Binding ClearFiltersCommand}" />

<Label Text="{Binding FilterSummary}" 
       IsVisible="{Binding CurrentFilter.HasFilters}" />
```

## Considerações de Performance

1. **Evite filtros muito amplos**: Configure `PageSize` apropriadamente
2. **Use ordenação quando necessário**: Ajuda na consistência dos resultados
3. **Cache filtros frequentes**: Salve filtros comuns para reutilização
4. **Monitore queries OData**: Use logs para identificar queries lentas

## Próximos Passos

- [ ] Implementar salvamento de filtros favoritos
- [ ] Adicionar suporte a ordenação por múltiplos campos
- [ ] Implementar validação de filtros incompatíveis
- [ ] Adicionar filtros por campos customizados
- [ ] Criar UI de filtro avançado reutilizável

---

**Documentação relacionada:**
- [README_BaseClasses.md](README_BaseClasses.md) - Arquitetura base de entidades e serviços
- [README_ApiResponse.md](README_ApiResponse.md) - Wrapper genérico de respostas da API
