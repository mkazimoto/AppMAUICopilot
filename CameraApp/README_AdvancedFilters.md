# Filtros Avançados - Interface Otimizada

## Visão Geral

Implementação de uma interface otimizada para filtros de formulários, com busca rápida por título na tela principal e filtros avançados em uma tela dedicada.

## Estrutura

### 1. FormListPage (Tela Principal)

**Filtro Simplificado:**
- ✅ Busca por título com ícone de lupa
- ✅ Botão "⚙️ Filtros" para abrir filtros avançados
- ✅ Botão "🔍" para executar busca
- ✅ Indicador de filtros ativos (ex: "🔍 3 filtros ativos")
- ✅ Botão "🗑️ Limpar" (visível apenas quando há filtros ativos)

**Interface Limpa:**
```
┌─────────────────────────────────────────────────────────┐
│ [🔍 Buscar por título...] [⚙️ Filtros] [🔍]           │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│ [🔍 3 filtros ativos]              [+ Novo] [🗑️ Limpar]│
└─────────────────────────────────────────────────────────┘
```

### 2. AdvancedFiltersPage (Filtros Avançados)

**Filtros Disponíveis:**

1. **Categoria** - Picker com opções predefinidas
2. **Status** - Picker com opções predefinidas
3. **Período de Criação** - DatePicker (De/Até)
4. **Criado Por** - Entry para nome do usuário
5. **Tipo de Script** - CheckBox "Apenas scripts sequenciais"
6. **Pontuação** - Entry (Mínimo/Máximo)
7. **Ordenação** - Picker com campos e CheckBox para ordem crescente

**Botões de Ação:**
- 🗑️ **Limpar Tudo** - Remove todos os filtros avançados
- ✓ **Aplicar Filtros** - Aplica filtros e volta para lista

## Fluxo de Uso

### Busca Rápida por Título

```csharp
1. Usuário digita na caixa de busca
2. Pressiona Enter ou clica no botão 🔍
3. SearchByTitleCommand é executado
4. FormFilter é criado apenas com o título
5. Lista é atualizada
6. Indicador mostra "🔍 1 filtro ativo"
```

### Filtros Avançados

```csharp
1. Usuário clica em "⚙️ Filtros"
2. AdvancedFiltersPage é aberta
3. Usuário configura múltiplos filtros
4. Clica em "✓ Aplicar Filtros"
5. ApplyAdvancedFiltersCommand é executado
6. FormFilter é construído com todos os critérios
7. Lista é atualizada
8. Página volta automaticamente para FormListPage
9. Indicador mostra quantidade de filtros ativos
```

### Limpar Filtros

```csharp
1. Usuário clica em "🗑️ Limpar"
2. ClearFiltersCommand é executado
3. Todos os campos são resetados
4. CurrentFilter = FormFilter.Default()
5. Lista é recarregada sem filtros
6. Indicador de filtros desaparece
```

## Propriedades do ViewModel

### Filtros Básicos (já existentes)
- `SelectedCategoryId` - Categoria selecionada
- `SelectedStatusId` - Status selecionado
- `StartDate` - Data inicial
- `EndDate` - Data final

### Novos Filtros
- `SearchTitle` - Texto da busca por título
- `CreatedBy` - Filtro por usuário criador
- `FilterSequentialScript` - Checkbox de script sequencial
- `MinScore` - Pontuação mínima
- `MaxScore` - Pontuação máxima
- `OrderByIndex` - Índice do campo de ordenação
- `OrderAscending` - Direção da ordenação

### Indicadores de Estado
- `CurrentFilter` - Objeto FormFilter atual
- `HasActiveFilters` - Boolean indicando filtros ativos
- `ActiveFiltersCount` - Contagem de filtros ativos

## Comandos

### Novos Comandos
- `SearchByTitleCommand` - Busca apenas por título
- `OpenAdvancedFiltersCommand` - Abre tela de filtros avançados
- `ApplyAdvancedFiltersCommand` - Aplica filtros avançados e volta
- `ClearAdvancedFiltersCommand` - Limpa apenas campos avançados

### Comandos Atualizados
- `ClearFiltersCommand` - Agora limpa TODOS os filtros (incluindo título)

### Comandos Removidos
- ❌ `FilterByCategoryCommand` (substituído por ApplyAdvancedFiltersCommand)
- ❌ `FilterByStatusCommand` (substituído por ApplyAdvancedFiltersCommand)
- ❌ `FilterByDateRangeCommand` (substituído por ApplyAdvancedFiltersCommand)

## Métodos Auxiliares

### `GetOrderByField()`
Converte o índice do picker em nome do campo:
```csharp
0 => "title"
1 => "recCreatedOn"
2 => "recModifiedOn"
3 => "totalScore"
4 => "categoryId"
```

### `UpdateFilterIndicators()`
Atualiza os indicadores de filtros ativos:
```csharp
- Conta quantos filtros estão ativos
- Atualiza HasActiveFilters (bool)
- Atualiza ActiveFiltersCount (int)
```

## Integração com FormFilter

### Busca por Título
```csharp
CurrentFilter = new FormFilter
{
    Title = SearchTitle,
    Page = CurrentPage,
    PageSize = 10
};
```

### Filtros Avançados Completos
```csharp
CurrentFilter = new FormFilter
{
    Title = SearchTitle,
    CategoryId = SelectedCategoryId > 0 ? SelectedCategoryId : null,
    StatusFormId = SelectedStatusId > 0 ? SelectedStatusId : null,
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
```

## Navegação

### Registro de Rotas (AppShell.xaml.cs)
```csharp
Routing.RegisterRoute(nameof(AdvancedFiltersPage), typeof(AdvancedFiltersPage));
```

### Navegação Modal
```csharp
// Abrir filtros avançados
await Shell.Current.GoToAsync(nameof(AdvancedFiltersPage));

// Voltar para lista (após aplicar)
await Shell.Current.GoToAsync("..");
```

## Injeção de Dependência

### MauiProgram.cs
```csharp
// AdvancedFiltersPage usa o mesmo ViewModel da lista
builder.Services.AddTransient<AdvancedFiltersPage>();
builder.Services.AddTransient<FormListViewModel>();
```

### AdvancedFiltersPage Constructor
```csharp
public AdvancedFiltersPage(FormListViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel; // Compartilha o mesmo ViewModel
}
```

## UX Melhorias

### Feedback Visual
- ✅ Ícones intuitivos (🔍, ⚙️, 🗑️, ✓)
- ✅ Indicador de filtros ativos sempre visível
- ✅ Botão limpar aparece/desaparece automaticamente
- ✅ Cores consistentes com tema da aplicação

### Performance
- ✅ Busca por título não carrega filtros avançados
- ✅ Filtros são aplicados apenas ao clicar "Aplicar"
- ✅ Indicadores atualizados de forma eficiente

### Usabilidade
- ✅ Enter na busca executa a pesquisa
- ✅ Página de filtros volta automaticamente após aplicar
- ✅ Estado dos filtros é preservado ao navegar
- ✅ Limpar filtros recarrega automaticamente

## Exemplo de Uso Completo

```csharp
// 1. Busca rápida
SearchTitle = "Inspeção";
await SearchByTitleCommand.ExecuteAsync(null);
// Resultado: Lista com formulários contendo "Inspeção" no título

// 2. Adicionar filtros avançados
await OpenAdvancedFiltersCommand.ExecuteAsync(null);
// Usuário configura: Categoria=2, Status=1, MinScore=70
await ApplyAdvancedFiltersCommand.ExecuteAsync(null);
// Resultado: Lista filtrada por título + categoria + status + pontuação

// 3. Limpar tudo
await ClearFiltersCommand.ExecuteAsync(null);
// Resultado: Lista completa sem filtros
```

## Vantagens da Nova Implementação

1. **Interface Limpa** - Tela principal focada no essencial
2. **Busca Rápida** - Acesso direto ao filtro mais usado
3. **Filtros Organizados** - Tela dedicada para configurações avançadas
4. **Feedback Visual** - Usuário sempre sabe quantos filtros estão ativos
5. **Reutilização** - Mesmo ViewModel para ambas as telas
6. **Flexibilidade** - Fácil adicionar novos filtros
7. **Manutenibilidade** - Código centralizado e bem organizado

---

**Arquivos Modificados:**
- ✅ `Views/FormListPage.xaml` - Interface simplificada
- ✅ `Views/AdvancedFiltersPage.xaml` - Nova tela de filtros
- ✅ `Views/AdvancedFiltersPage.xaml.cs` - Code-behind
- ✅ `ViewModels/FormListViewModel.cs` - Novos comandos e propriedades
- ✅ `AppShell.xaml.cs` - Registro de rota
- ✅ `MauiProgram.cs` - Registro DI

**Build:** ✅ Compilado com sucesso (3 avisos XAML, 0 erros)
