---
mode: agent
description: Gera testes xUnit completos para uma classe do projeto CameraApp (Converter, Service ou ViewModel), seguindo as convenções do projeto.
---

# Gerar Testes xUnit – CameraApp

Gere um arquivo de testes xUnit completo para a classe indicada abaixo, seguindo **todas** as convenções do projeto CameraApp.

## Classe a testar

```
${selection}
```

> Se nenhum trecho estiver selecionado use a classe do arquivo ativo atual.

---

## Regras obrigatórias

### Localização do arquivo
| Tipo da classe | Pasta do teste |
|---|---|
| `Converters/` | `CameraApp.Test/Converters/` |
| `Services/` | `CameraApp.Test/Services/` |
| `ViewModels/` | `CameraApp.Test/ViewModels/` |

Nome do arquivo: `{NomeDaClasse}Tests.cs`

### Namespace e usings
- Namespace: `CameraApp.Test.{Converters|Services|ViewModels}`
- **Não** adicione `using Xunit;` nem `using Moq;` — já estão em `GlobalUsings.cs`.
- Adicione apenas os `using` estritamente necessários para tipos externos.

### Estrutura da classe
```csharp
/// <summary>
/// Unit tests for <see cref="NomeDaClasse" />.
/// </summary>
public class NomeDaClasseTests
{
    // campos readonly para mocks e _sut
    private readonly Mock<IDependencia> _dependenciaMock = new();
    private readonly NomeDaClasse _sut;

    public NomeDaClasseTests()
    {
        _sut = new NomeDaClasse(_dependenciaMock.Object);
    }
    ...
}
```
- Para ViewModels com muitas dependências use um método `CreateSut()` em vez de atribuir no construtor.
- Nomeie o campo do sistema sob teste sempre como `_sut`.

### Organização dos testes
- Agrupe por método/comportamento com separadores de seção:
  ```csharp
  // ── NomeDoMétodo ──────────────────────────────────────────────────────────
  ```
- Nomeie cada teste: `MétodoOuProp_Cenário_ResultadoEsperado`
- Use `[Fact]` para cenários únicos e `[Theory]` + `[InlineData]` para múltiplas entradas.

### Padrão AAA
```csharp
// Arrange
...
// Act
var result = ...
// Assert
Assert.Equal(expected, result);
```

### Cenários obrigatórios — guia por tipo

**Converter (`IValueConverter`)**
- `Convert` com entrada válida → resultado esperado
- `Convert` com tipo inválido / `null` → valor padrão seguro
- `ConvertBack` quando aplicável

**Service**
- Caminho feliz (happy path)
- Dependência retorna `null`
- Dependência lança exceção → método retorna valor seguro ou lança exceção documentada
- Verificação de chamadas com `Verify()`

**ViewModel**
- Estado inicial de cada `ObservableProperty`
- `CanExecute` de cada `RelayCommand` (valores que habilitam e desabilitam)
- Execução do comando: caminho feliz, falha de serviço, `IsBusy` durante execução
- Navegação chamada quando esperado

### Qualidade
- Cada `[Fact]` deve testar **um único comportamento**.
- Não teste detalhes de implementação internos, apenas contratos públicos.
- Use `Assert.NotNull` antes de acessar propriedades de resultado que podem ser `null`.
- Para testes assíncronos use `async Task`, nunca `async void`.

---

Gere o arquivo completo, pronto para ser criado em `CameraApp.Test/{pasta}/{NomeDaClasse}Tests.cs`.
