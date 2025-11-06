# Classes Base Genéricas - Arquitetura do Projeto

Este documento descreve as classes base genéricas implementadas para padronizar modelos e serviços REST no projeto.

## Estrutura

### 1. BaseEntity (Models/BaseEntity.cs)

Classe abstrata base para todas as entidades do sistema.

**Propriedades:**
- `Id` (string): Identificador único da entidade
- `RecCreatedOn` (DateTime?): Data de criação do registro
- `RecCreatedBy` (string?): Usuário que criou o registro
- `RecModifiedOn` (DateTime?): Data da última modificação
- `RecModifiedBy` (string?): Usuário que modificou o registro

**Exemplo de uso:**
```csharp
public class Product : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}
```

### 2. IBaseService<T> (Services/IBaseService.cs)

Interface genérica que define operações CRUD padrão para serviços REST.

**Métodos:**
- `GetAllAsync(page, pageSize)`: Obtém lista paginada de entidades
- `GetByIdAsync(id)`: Obtém uma entidade por ID
- `CreateAsync(entity)`: Cria uma nova entidade
- `UpdateAsync(id, entity)`: Atualiza uma entidade existente
- `DeleteAsync(id)`: Exclui uma entidade

### 3. BaseService<T> (Services/BaseService.cs)

Implementação base genérica para serviços REST com:
- ✅ Autenticação automática via IAuthService
- ✅ Renovação automática de token
- ✅ Tratamento de erros padronizado
- ✅ Serialização/Deserialização JSON
- ✅ Suporte a paginação
- ✅ Logs de debug

**Propriedade abstrata:**
- `EndpointPath`: Deve ser implementada para definir o endpoint da API

### 4. PaginatedResponse<T> (Services/IBaseService.cs)

Classe genérica para respostas paginadas da API.

**Propriedades:**
- `Items`: Lista de itens da página atual
- `Page`: Página atual
- `PageSize`: Tamanho da página
- `TotalCount`: Total de itens disponíveis
- `TotalPages`: Total de páginas (calculado)
- `HasPreviousPage`: Indica se há página anterior
- `HasNextPage`: Indica se há próxima página

### 5. ApiResponse<T> (Models/ApiResponse.cs)

Classe base genérica para respostas da API TOTVS que retornam listas.

**Propriedades:**
- `HasNext` (bool): Indica se há mais itens disponíveis
- `Items` (List<T>): Lista de itens retornados
- `Count` (int, calculado): Quantidade de itens retornados
- `IsEmpty` (bool, calculado): Indica se a resposta está vazia

**Exemplo de uso:**
```csharp
// Resposta específica para formulários (compatibilidade)
public class FormResponse : ApiResponse<Form> { }

// Ou criar novas respostas tipadas
public class CustomerResponse : ApiResponse<Customer> { }
public class ProductResponse : ApiResponse<Product> { }
```

## Como Criar um Novo Serviço

### Passo 1: Criar o Modelo

```csharp
using CameraApp.Models;

namespace CameraApp.Models;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
```

### Passo 2: Criar a Interface do Serviço (Opcional)

```csharp
using CameraApp.Services;

namespace CameraApp.Services;

public interface ICustomerService : IBaseService<Customer>
{
    // Métodos adicionais específicos do serviço
    Task<List<Customer>> GetByEmailAsync(string email);
}
```

### Passo 3: Implementar o Serviço

```csharp
using CameraApp.Services;
using CameraApp.Models;

namespace CameraApp.Services;

public class CustomerService : BaseService<Customer>, ICustomerService
{
    // Define o endpoint da API
    protected override string EndpointPath => "/api/customers";

    public CustomerService(HttpClient httpClient, IAuthService authService) 
        : base(httpClient, authService)
    {
    }

    // Implementar métodos adicionais específicos
    public async Task<List<Customer>> GetByEmailAsync(string email)
    {
        await EnsureAuthenticatedAsync();
        
        var url = $"{Config.ApiConfig.BaseUrl}{EndpointPath}?$filter=email eq '{email}'";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(
                jsonResponse, 
                GetJsonOptions()
            ) ?? new List<Customer>();
        }
        
        await HandleErrorResponseAsync(response);
        return new List<Customer>();
    }
}
```

### Passo 4: Registrar no MauiProgram.cs

```csharp
// Registrar o serviço
builder.Services.AddSingleton<ICustomerService, CustomerService>();
```

### Passo 5: Usar no ViewModel

```csharp
public class CustomerListViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;

    public CustomerListViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
        try
        {
            // Obtém lista paginada
            var response = await _customerService.GetAllAsync(page: 1, pageSize: 20);
            
            Customers = new ObservableCollection<Customer>(response.Items);
            
            // Verifica se há mais páginas
            if (response.HasNextPage)
            {
                // Carregar próxima página
            }
        }
        catch (ApiException ex)
        {
            // Tratar erro
            await ShowErrorAsync(ex.Message);
        }
    }

    [RelayCommand]
    public async Task CreateCustomerAsync()
    {
        try
        {
            var newCustomer = new Customer
            {
                Name = "João Silva",
                Email = "joao@example.com",
                Phone = "(11) 98765-4321"
            };

            var created = await _customerService.CreateAsync(newCustomer);
            
            if (created != null)
            {
                Customers.Add(created);
            }
        }
        catch (ApiException ex)
        {
            await ShowErrorAsync(ex.Message);
        }
    }
}
```

## Recursos Automáticos

### 1. Autenticação Automática
Todas as chamadas verificam e renovam o token automaticamente antes de executar.

### 2. Tratamento de Erros
- Erros 401: Tenta renovar o token automaticamente
- Outros erros: Deserializa ApiError do TOTVS ou cria erro genérico
- Exceções: Converte para ApiException com mensagens apropriadas

### 3. Logs de Debug
Todos os serviços incluem logs detalhados:
```
[BaseService<Customer>] Error - Status: 404
[BaseService<Customer>] Error Content: {"code":"FE404","message":"Cliente não encontrado"}
```

### 4. Serialização JSON
Opções padrão configuradas:
- `PropertyNameCaseInsensitive`: true
- `PropertyNamingPolicy`: CamelCase

## Métodos Protegidos Disponíveis

Para serviços que herdam de `BaseService<T>`:

- `EnsureAuthenticatedAsync()`: Garante autenticação válida
- `HandleErrorResponseAsync(response)`: Trata erros da API
- `GetJsonOptions()`: Retorna opções de serialização JSON

## Exemplo Completo: FormServiceGeneric

Veja o arquivo `Services/FormServiceGeneric.cs` para um exemplo completo de implementação com métodos customizados como:
- `GetByCategoryAsync(categoryId)`
- `GetByStatusAsync(statusId)`

## Benefícios

✅ **Redução de código duplicado**: Operações CRUD já implementadas
✅ **Consistência**: Padrão único para todos os serviços
✅ **Manutenibilidade**: Mudanças centralizadas na classe base
✅ **Type Safety**: Generics garantem tipos corretos
✅ **Reutilização**: Fácil criar novos serviços
✅ **Testabilidade**: Interface permite mocks fáceis

## Migração de Serviços Existentes

Para migrar um serviço existente (como FormService):

1. Fazer o modelo herdar de `BaseEntity`
2. Criar nova classe herdando de `BaseService<T>`
3. Implementar apenas a propriedade `EndpointPath`
4. Mover métodos customizados (filtros, etc.)
5. Remover código duplicado (CRUD já está na base)
6. Atualizar registro no MauiProgram.cs

## Observações Importantes

- A propriedade `Id` em `BaseEntity` é do tipo `string` para compatibilidade com APIs que usam GUIDs
- A paginação segue o padrão TOTVS (page começa em 1)
- Todos os métodos são `virtual` e podem ser sobrescritos se necessário
- O tratamento de erro é compatível com o formato de erro da API TOTVS
