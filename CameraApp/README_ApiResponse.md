# Usando ApiResponse<T> - Exemplos Práticos

## Visão Geral

A classe `ApiResponse<T>` é uma classe base genérica para respostas de API que retornam listas de itens. Ela é compatível com o formato de resposta da API TOTVS e pode ser reutilizada para qualquer tipo de entidade.

## Estrutura da Resposta da API TOTVS

```json
{
  "hasNext": true,
  "items": [
    {
      "id": "1",
      "title": "Formulário 1",
      "categoryId": 1,
      ...
    },
    {
      "id": "2",
      "title": "Formulário 2",
      "categoryId": 2,
      ...
    }
  ]
}
```

## Usando ApiResponse<T>

### 1. Para Formulários (Já Implementado)

```csharp
// A classe FormResponse herda de ApiResponse<Form> para compatibilidade
public class FormResponse : ApiResponse<Form> { }

// Uso no serviço
var response = await _httpClient.GetAsync(url);
var formResponse = JsonSerializer.Deserialize<FormResponse>(jsonResponse);

// Acessar dados
if (formResponse.HasNext)
{
    // Há mais páginas disponíveis
}

var forms = formResponse.Items; // Lista de formulários
var count = formResponse.Count; // Quantidade de itens
var isEmpty = formResponse.IsEmpty; // Verifica se está vazia
```

### 2. Para Novos Tipos de Entidade

#### Exemplo com Clientes

```csharp
// Model
public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

// Response (opcional - para compatibilidade de nomenclatura)
public class CustomerResponse : ApiResponse<Customer> { }

// Uso direto sem criar classe específica
var customerResponse = JsonSerializer.Deserialize<ApiResponse<Customer>>(jsonResponse);
```

#### Exemplo com Produtos

```csharp
// Model
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

// Uso direto
var productResponse = JsonSerializer.Deserialize<ApiResponse<Product>>(jsonResponse);

foreach (var product in productResponse.Items)
{
    Console.WriteLine($"{product.Name}: R$ {product.Price}");
}
```

## Vantagens da Classe Genérica

### ✅ Reutilização
Uma única classe serve para todos os tipos de entidades.

```csharp
// Antes (código duplicado)
public class FormResponse { ... }
public class CustomerResponse { ... }
public class ProductResponse { ... }

// Depois (reutilização)
var formResponse = new ApiResponse<Form>();
var customerResponse = new ApiResponse<Customer>();
var productResponse = new ApiResponse<Product>();
```

### ✅ Type Safety
O compilador garante que você está trabalhando com o tipo correto.

```csharp
ApiResponse<Form> formResponse = ...;
// formResponse.Items é do tipo List<Form>

ApiResponse<Customer> customerResponse = ...;
// customerResponse.Items é do tipo List<Customer>
```

### ✅ Propriedades Calculadas
Propriedades úteis já disponíveis.

```csharp
var response = await GetDataAsync();

// Verificar se há dados
if (response.IsEmpty)
{
    ShowMessage("Nenhum dado encontrado");
    return;
}

// Contar itens
ShowMessage($"Encontrados {response.Count} itens");

// Verificar se há mais páginas
if (response.HasNext)
{
    ShowLoadMoreButton();
}
```

## Integração com BaseService<T>

O `BaseService<T>` usa automaticamente `ApiResponse<T>` para deserializar respostas:

```csharp
public class CustomerService : BaseService<Customer>
{
    protected override string EndpointPath => "/api/customers";

    // Método customizado usando ApiResponse
    public async Task<List<Customer>> GetActiveCustomersAsync()
    {
        await EnsureAuthenticatedAsync();
        
        var url = $"{ApiConfig.BaseUrl}{EndpointPath}?$filter=active eq true";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            // Deserializa como ApiResponse<Customer>
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<Customer>>(
                jsonResponse, 
                GetJsonOptions()
            );
            
            return apiResponse?.Items ?? new List<Customer>();
        }
        
        await HandleErrorResponseAsync(response);
        return new List<Customer>();
    }
}
```

## Convertendo para PaginatedResponse

O `BaseService<T>` converte automaticamente `ApiResponse<T>` para `PaginatedResponse<T>`:

```csharp
public override async Task<PaginatedResponse<T>> GetAllAsync(int page = 1, int pageSize = 10)
{
    // ...
    
    // Deserializa como ApiResponse<T>
    var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(jsonResponse);
    
    // Converte para PaginatedResponse<T>
    return new PaginatedResponse<T>
    {
        Items = apiResponse.Items,
        Page = page,
        PageSize = pageSize,
        TotalCount = apiResponse.Items.Count
    };
}
```

## Migração de Código Existente

### FormService Antigo
```csharp
// Antes
public class FormResponse
{
    public bool HasNext { get; set; }
    public List<Form> Items { get; set; } = new();
}

var formResponse = JsonSerializer.Deserialize<FormResponse>(json);
```

### FormService Novo
```csharp
// Depois - opção 1: usar classe específica
public class FormResponse : ApiResponse<Form> { }
var formResponse = JsonSerializer.Deserialize<FormResponse>(json);

// Depois - opção 2: usar diretamente
var formResponse = JsonSerializer.Deserialize<ApiResponse<Form>>(json);

// Novos recursos disponíveis
if (formResponse.IsEmpty) { ... }
var count = formResponse.Count;
```

## Casos de Uso Comuns

### 1. Verificar se há resultados
```csharp
var response = await _service.GetAllAsync();
if (response.IsEmpty)
{
    ShowEmptyState();
    return;
}
```

### 2. Implementar Load More
```csharp
var response = await GetPageAsync(currentPage);
Items.AddRange(response.Items);

if (response.HasNext)
{
    LoadMoreButton.IsVisible = true;
}
```

### 3. Exibir contadores
```csharp
var response = await SearchAsync(query);
ResultCountLabel.Text = $"Encontrados {response.Count} resultados";
```

## Boas Práticas

### ✅ Use ApiResponse<T> para:
- Deserializar respostas da API TOTVS
- Verificar se há mais páginas (`HasNext`)
- Obter contagem rápida de itens (`Count`)
- Verificar se a lista está vazia (`IsEmpty`)

### ✅ Use PaginatedResponse<T> para:
- ViewModels que precisam de informações de paginação
- Calcular total de páginas
- Implementar navegação entre páginas

### ✅ Crie classes específicas (FormResponse) apenas para:
- Manter compatibilidade com código existente
- Adicionar métodos ou propriedades específicas
- Melhorar legibilidade em casos específicos

## Resumo

A transformação de `FormResponse` em uma classe base genérica `ApiResponse<T>` traz:

- ✅ **Menos código duplicado**: Uma classe para todas as entidades
- ✅ **Maior consistência**: Mesmo padrão em todo o projeto
- ✅ **Type safety**: Compilador garante tipos corretos
- ✅ **Facilidade de manutenção**: Mudanças em um único lugar
- ✅ **Propriedades úteis**: Count, IsEmpty já implementados
- ✅ **Compatibilidade**: FormResponse ainda existe herdando da base
