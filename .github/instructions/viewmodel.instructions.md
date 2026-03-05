---
description: "Use when creating, refactoring, or reviewing MAUI ViewModels with CommunityToolkit.Mvvm. Covers DI, RelayCommand, ObservableProperty, async flows, validation, navigation, and UI-state patterns."
applyTo: "**/ViewModels/*.cs"
---

# Instrucoes para ViewModels (MAUI)

Ao gerar ou sugerir codigo em `ViewModels`, use estas convencoes como guideline preferencial do projeto:

## 1. Estrutura e base MVVM
- Herde de `ObservableObject` e mantenha a classe `partial` quando usar source generators do CommunityToolkit.
- Prefira `[ObservableProperty]` para estado mutavel e `[RelayCommand]` para acoes de UI.
- Nomeie ViewModels em PascalCase com sufixo `ViewModel`.

## 2. Dependencias e responsabilidade
- Injete dependencias via construtor (servicos, logger, provedores de navegacao).
- Nao instancie services diretamente dentro do ViewModel.
- Nao coloque chamadas HTTP, acesso a banco, ou detalhes de infraestrutura no ViewModel; delegue para `Services`.

## 3. Comandos e assinaturas assincronas
- Comandos que fazem I/O devem ser assincronos com `Task` e sufixo `Async`.
- Use guard clauses para evitar reentrada (`if (IsLoading) return;`).
- Controle estados de carregamento com `try/finally` para garantir reset de `IsLoading`.

## 4. Estado de UI e validacao
- Exponha estados derivados como propriedades calculadas (`CanSave`, `HasErrors`, etc.).
- Dispare `OnPropertyChanged` para propriedades derivadas quando campos relacionados mudarem.
- Mantenha validacoes de entrada no ViewModel e mensagens de erro em propriedades bindaveis.

## 5. Navegacao e interacao com UI
- Prefira navegacao via `Shell.Current.GoToAsync` com rotas nomeadas e parametros explicitos.
- Nao manipule controles diretamente; use binding.
- Nao chame APIs de UI diretamente no ViewModel (`DisplayAlert`, `Application.Current`, `MainThread`, etc.).
- Para dialogs, confirmacoes e feedback visual, use uma abstracao injetada (ex.: `IDialogService`/`IUiDispatcher`) para manter testabilidade.

## 6. Tratamento de erros e observabilidade
- Nao silencie excecoes; trate e publique feedback para a UI (`ErrorMessage`) quando aplicavel.
- Use `ILogger<TViewModel>` para diagnostico em vez de `Console.WriteLine` ou `Debug.WriteLine`.
- Em erros previsiveis (API, validacao, autorizacao), retorne mensagens orientadas ao usuario e preserve detalhes tecnicos no log.

## 7. Documentacao e estilo
- Documente membros publicos nao triviais com comentarios XML (`///`).
- Prefira `file-scoped namespace` quando consistente com o arquivo.
- Use nomes claros: PascalCase para membros publicos e `_camelCase` para campos privados.
