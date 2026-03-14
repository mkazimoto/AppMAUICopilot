---
name: create-service-rest-maui
description: >
  Cria um serviço REST completo neste projeto .NET MAUI — Model (herda BaseEntity), interface,
  implementação (herda BaseService<T> para CRUD padrão, ou customizada para APIs não-padrão),
  constante de endpoint em ApiConfig, e registro de DI em MauiProgram.cs.
  Use sempre que o usuário pedir: "criar serviço", "add a REST service", "criar serviço para [entidade]",
  "preciso de um serviço para [entidade]", "criar CRUD para [recurso]", "integrar com [API]",
  "criar [entidade] service", "create service for [entity]", "new service", "add service",
  "serviço REST para [recurso]", "quero consumir a API de [recurso]".
---

# Skill: create-service-rest-maui

## Objetivo

Gerar todos os artefatos necessários para um serviço REST neste projeto MAUI, seguindo as
convenções existentes: contrato de interface, implementação tipada, endpoint centralizado e
registro no container de DI.

## Entradas Esperadas

- **Nome da entidade** (ex.: `Product`, `Customer`, `Order`)
- **Caminho relativo do endpoint** (ex.: `/api/construction-projects/v1/products`)
- **Variante**:
  - `generic` — herda `BaseService<T>`, CRUD padrão pronto
  - `custom` — implementa interface diretamente (para APIs com formato não-padrão)
- O model já existe? (`sim` / `não`)

Se o usuário não informar variante, perguntar se o CRUD é padrão (GET list/id, POST, PUT, DELETE)
ou se a API tem formatos especiais (filtros compostos, respostas distintas, etc.).

---

## Workflow

### 1. Validar contexto

- Confirmar pastas `Models/`, `Services/`, `Config/ApiConfig.cs` e `MauiProgram.cs` presentes.
- Verificar se já existe `I{EntityName}Service.cs` ou `{EntityName}Service.cs` para evitar duplicatas.

### 2. Criar Model (se não existir)

Arquivo: `Models/{EntityName}.cs`

```csharp
namespace CameraApp.Models;

/// <summary>
/// Represents a {entityName} entity.
/// </summary>
public class {EntityName} : BaseEntity
{
    // TODO: adicionar propriedades específicas da entidade
    // Exemplo:
    // public string Name { get; set; } = string.Empty;
}
```

- Herdar de `BaseEntity` (já tem `Id`, `RecCreatedOn`, `RecCreatedBy`, `RecModifiedOn`, `RecModifiedBy`).
- Usar `file-scoped namespace`.
- Documentar com XML comments (`///`).

### 3. Adicionar Endpoint em ApiConfig

Arquivo: `Config/ApiConfig.cs` — classe interna `Endpoints`:

```csharp
public const string {EntityName}s = "/api/.../v1/{entityNames}";
```

Use o caminho fornecido pelo usuário. Mantenha o padrão de constantes existentes.

### 4a. Variante Generic — BaseService\<T\>

Arquivo: `Services/{EntityName}Service.cs`

```csharp
namespace CameraApp.Services;

/// <summary>
/// Provides CRUD operations for <see cref="Models.{EntityName}"/> entities.
/// </summary>
public class {EntityName}Service : BaseService<Models.{EntityName}>, I{EntityName}Service
{
    /// <inheritdoc/>
    protected override string EndpointPath => ApiConfig.Endpoints.{EntityName}s;

    /// <summary>
    /// Initializes a new instance of <see cref="{EntityName}Service"/>.
    /// </summary>
    public {EntityName}Service(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService) { }

    // Adicione métodos específicos aqui se necessário
}
```

Arquivo: `Services/I{EntityName}Service.cs`

```csharp
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines CRUD operations for <see cref="{EntityName}"/> entities.
/// </summary>
public interface I{EntityName}Service : IBaseService<{EntityName}>
{
    // Adicione contratos específicos aqui se necessário
}
```

> Use esta variante quando o endpoint segue GET/GET-by-id/POST/PUT/DELETE padrão.
> `BaseService<T>` já implementa toda a lógica — o serviço herda tudo automaticamente.

### 4b. Variante Custom

Arquivo: `Services/I{EntityName}Service.cs`

```csharp
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines operations for managing <see cref="{EntityName}"/> entities via the API.
/// </summary>
public interface I{EntityName}Service
{
    Task<PaginatedResponse<{EntityName}>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<{EntityName}?> GetByIdAsync(string id);
    Task<{EntityName}?> CreateAsync({EntityName} entity);
    Task<{EntityName}?> UpdateAsync(string id, {EntityName} entity);
    Task<bool> DeleteAsync(string id);
    // Adicione contratos específicos da API aqui
}
```

Arquivo: `Services/{EntityName}Service.cs`

```csharp
using System.Text;
using System.Text.Json;
using CameraApp.Config;
using CameraApp.Exceptions;
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Provides operations for <see cref="{EntityName}"/> entities against the API.
/// </summary>
public class {EntityName}Service : I{EntityName}Service
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of <see cref="{EntityName}Service"/>.
    /// </summary>
    public {EntityName}Service(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResponse<{EntityName}>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}{ApiConfig.Endpoints.{EntityName}s}?page={page}&pagesize={pageSize}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<{EntityName}>>(json, ApiConfig.GetJsonOptions());
                return new PaginatedResponse<{EntityName}>
                {
                    Items = items ?? new(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = items?.Count ?? 0
                };
            }

            await HandleErrorResponseAsync(response);
            return new PaginatedResponse<{EntityName}>();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw new ApiException($"Erro ao carregar {EntityName}s: {ex.Message}", ex);
        }
    }

    // Implemente GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync seguindo
    // o mesmo padrão: try/catch, HandleErrorResponseAsync para não-2xx,
    // re-throw ApiException, wrap demais em new ApiException(...)

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var apiError = string.IsNullOrEmpty(content)
            ? null
            : JsonSerializer.Deserialize<ApiError>(content, ApiConfig.GetJsonOptions());
        throw new ApiException(
            apiError?.Message ?? $"Erro HTTP {(int)response.StatusCode}",
            (int)response.StatusCode);
    }
}
```

> Use esta variante quando a API tem formato de resposta não-padrão, filtros compostos, ou
> precisa de endpoints extras além do CRUD básico.

### 5. Registrar no DI — MauiProgram.cs

Localize o bloco de registro de serviços e adicione **após** os serviços existentes:

```csharp
builder.Services.AddSingleton<I{EntityName}Service, {EntityName}Service>();
```

Use `AddSingleton` para serviços stateless (padrão neste projeto). Use `AddTransient` apenas se
o serviço mantiver estado por requisição.

### 6. Validar Build

```bash
dotnet build -f net10.0-android
```

Verificar antes do build:
- Todos `StaticResource` utilizados existem nos estilos.
- Namespaces corretos (`CameraApp.Services`, `CameraApp.Models`, `CameraApp.Config`).
- Interface registrada no DI (`I{EntityName}Service`).

---

## Decisões e Ramificações

| Situação | Ação |
|---|---|
| API retorna CRUD padrão | Usar variante `generic` (BaseService<T>) — menos código, mesmo comportamento |
| API tem filtros ou formatos distintos | Usar variante `custom` e implementar os métodos necessários |
| Model já existe no projeto | Pular passo 2 |
| Endpoint já em ApiConfig | Pular passo 3 |
| Serviço terá estado entre calls | Registrar como `AddTransient`, não `AddSingleton` |

---

## Critérios de Qualidade

- Build sem erros.
- Interface registrada no DI e injetável em ViewModels.
- Nenhuma URL, timeout ou header hardcoded fora de `ApiConfig`.
- `ApiException` lançada com contexto suficiente (operação + status + mensagem).
- Todos os membros públicos documentados com XML comments (`///`).
- Nenhuma lógica de UI nos Services.
