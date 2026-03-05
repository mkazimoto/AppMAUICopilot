---
name: criar-pagina-maui
description: Cria uma nova pagina .NET MAUI com View, code-behind e ViewModel em padrao MVVM, incluindo rota Shell, DI e validacao basica.
---

# Skill: Criar Pagina MAUI

## Objetivo

Gerar uma pagina MAUI pronta para uso no projeto, mantendo padrao MVVM, navegacao via Shell, nomenclatura consistente e separacao entre UI e logica.

## Entradas Esperadas

- Nome da pagina (ex.: `PerfilPage`)
- Nome do ViewModel (opcional, default: `<Pagina>ViewModel`)
- Tipo de layout principal (`VerticalStackLayout`, `Grid`, `ScrollView`) - padrao: `Form`
- Necessita rota no Shell? (`sim`/`nao`)
- Necessita servicos via DI? (`sim`/`nao`)

## Workflow

1. Validar contexto do projeto
- Confirmar que o projeto e MAUI e possui pasta `Views/` e `ViewModels/`.
- Confirmar convencao existente no repositorio para nomes e rotas.

2. Definir nomes e arquivos
- Criar `Views/<Pagina>.xaml`.
- Criar `Views/<Pagina>.xaml.cs`.
- Criar `ViewModels/<Pagina>ViewModel.cs` (ou nome informado).

3. Criar View XAML
- Definir `x:Class` correto.
- Definir `x:DataType` para compiled bindings.
- Aplicar template padrao `Form` quando o usuario nao especificar outro tipo.
- Usar layout simples e sem aninhamento excessivo.
- Usar `StaticResource` para estilos, evitando inline repetitivo.
- Incluir `SemanticProperties` quando houver elementos interativos.

4. Criar code-behind minimo
- Construtor recebe ViewModel por injecao (quando aplicavel).
- Definir `BindingContext = viewModel` apenas se padrao do projeto exigir.
- Nao adicionar logica de negocio no code-behind.

5. Criar ViewModel
- Herdar de base do projeto (se houver) ou de `ObservableObject`.
- Criar propriedades observaveis e comandos (`[RelayCommand]`) necessarios.
- Injetar servicos por interface quando houver dependencia externa.
- Usar `async/await` para I/O e lidar com falhas de forma previsivel.

6. Integrar navegacao e DI
- Registrar rota no `AppShell`/`MainShell` quando solicitado.
- Registrar ViewModel e servicos no `MauiProgram.cs`.
- Garantir que navegacao por rota compile e resolva dependencias.

7. Validar compilacao
- Verificar referencias e namespaces.
- Verificar `StaticResource` existente.
- Rodar build Android:

```bash
dotnet build -t:Run -f net10.0-android
```

## Decisoes e Ramificacoes

- Se o usuario nao informar template:
  Assumir pagina de `Form` como padrao.
- Se a pagina tiver formulario extenso:
  Use `ScrollView` + `VerticalStackLayout`.
- Se a pagina for lista de itens:
  Prefira `CollectionView`.
- Se houver dependencia de API/camera/localizacao:
  Criar/injetar servico por interface e nao acessar API diretamente na View.
- Se nao houver necessidade de estado complexo:
  ViewModel minimo com comandos essenciais.

## Criterios de Qualidade

- Build sem erros.
- Pagina abre via Shell sem excecao.
- Nenhuma logica de negocio no code-behind.
- Bindings resolvidos (sem warnings de binding criticos).
- Estilo visual consistente com recursos do projeto.

## Checklist de Conclusao

- [ ] Arquivos de View e ViewModel criados.
- [ ] Namespace e `x:Class` corretos.
- [ ] Rota Shell registrada (quando solicitado).
- [ ] DI configurada para ViewModel/servicos.
- [ ] Build Android executado com sucesso.

## Exemplos de Prompt

- "Use a skill `criar-pagina-maui` para criar `PerfilPage` com campos nome e email."
- "Use `criar-pagina-maui` para criar `HistoricoPage` com `CollectionView` e filtro."
- "Aplique `criar-pagina-maui` para gerar `ConfigPage` e registrar rota no Shell."
