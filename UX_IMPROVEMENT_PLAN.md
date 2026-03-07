# Plano de Melhorias UX — CameraApp .NET MAUI

Gerado em: 07/03/2026  
Baseado na análise do checklist UX do agente `especialista-ux`.

---

## Legenda de Prioridade

| Símbolo | Prioridade | Critério |
|---|---|---|
| 🔴 | Crítico | Bloquear entrega — erros visuais graves, tokens ausentes, aninhamento excessivo |
| 🟡 | Aviso | Corrigir antes do próximo sprint — acessibilidade, automação, inconsistências |
| 🔵 | Sugestão | Melhoria de qualidade — refatoração, semântica, performance de layout |

---

## PASSO 1 — Tokens de cor ausentes em Colors.xaml 🔴

**Arquivo:** `CameraApp/Resources/Styles/Colors.xaml`

- [ ] Adicionar `<Color x:Key="Danger">#D32F2F</Color>`
- [ ] Adicionar `<Color x:Key="Success">#388E3C</Color>`
- [ ] Adicionar `<SolidColorBrush x:Key="DangerBrush" Color="{StaticResource Danger}"/>`
- [ ] Adicionar `<SolidColorBrush x:Key="SuccessBrush" Color="{StaticResource Success}"/>`

> **Motivo:** `LoginPage`, `CameraPage` e `PosturePage` referenciam `Danger` e `Success` via `DynamicResource`/hardcode. Sem os tokens, a UI pode exibir fallback silencioso ou crash em release.

---

## PASSO 2 — LoginPage.xaml 🔴🟡🔵

**Arquivo:** `CameraApp/Views/LoginPage.xaml`

### 🔴 Crítico
- [ ] Corrigir `TextColor="Red"` para `TextColor="White"` dentro do `Border` de erro (fundo `Danger`)
- [ ] Substituir `TextColor="White"` hardcoded por `TextColor="{StaticResource White}"` (3 ocorrências)

### 🟡 Aviso
- [ ] Adicionar `AutomationId` nos controles interativos:
  - `Entry` username → `AutomationId="UsernameEntry"`
  - `Entry` password → `AutomationId="PasswordEntry"`
  - `Entry` serviceAlias → `AutomationId="ServiceAliasEntry"`
  - `Button` Entrar → `AutomationId="LoginButton"`
  - `Button` Limpar → `AutomationId="ClearButton"`
  - `Button` Sair → `AutomationId="LogoutButton"`
- [ ] Adicionar `SemanticProperties.Description` nos botões
- [ ] Mover bloco de mensagem de erro para dentro do `Border` do formulário (Row 1), logo acima dos botões, para proximidade visual com o contexto

### 🔵 Sugestão
- [ ] Substituir `StackLayout` genérico por `VerticalStackLayout` (6 ocorrências)
- [ ] Adicionar `SemanticProperties.HeadingLevel="Level2"` no Label "Dados de Acesso"

---

## PASSO 3 — FormListPage.xaml 🔴🟡🔵

**Arquivo:** `CameraApp/Views/FormListPage.xaml`

### 🔴 Crítico
- [ ] Substituir `BackgroundColor="White"` hardcoded por `{StaticResource White}` (2 ocorrências no `DataTemplate`)
- [ ] Remover `StackLayout` redundante dentro de `Border > StackLayout` no `DataTemplate` — mover o conteúdo (Grid) direto para o `Border`, eliminando 2 níveis de aninhamento

### 🟡 Aviso
- [ ] Adicionar `SemanticProperties.Description` nos botões emoji (🔍)
- [ ] Adicionar `SemanticProperties.Description` no `TapGestureRecognizer` do item de lista
- [ ] Adicionar `AutomationId` em:
  - `Entry` de busca → `AutomationId="SearchEntry"`
  - `Button` buscar → `AutomationId="SearchButton"`
  - `Button` Filtros Avançados → `AutomationId="AdvancedFiltersButton"`
  - `Button` Novo → `AutomationId="NewFormButton"`
  - `Button` Limpar Filtros → `AutomationId="ClearFiltersButton"`
- [ ] Padronizar `CornerRadius="8"` nos botões de ação (atualmente mistura `8` e `5`)

### 🔵 Sugestão
- [ ] Adicionar feedback visual de toque no `Border` do item de lista via `VisualStateManager`

---

## PASSO 4 — FormEditPage.xaml 🔴🟡🔵

**Arquivo:** `CameraApp/Views/FormEditPage.xaml`

### 🔴 Crítico
- [ ] Remover `StackLayout` internos redundantes nas seções que duplicam `BackgroundColor` e `Padding` já definidos no `Border` pai — afeta 4 seções:
  - Header (Primary)
  - Seções de campo com `BackgroundColor="White" Padding="15"` (4×)
- [ ] Remover `IsVisible` e `BackgroundColor` duplicados do `StackLayout` interno na seção "Informações de ID"
- [ ] Substituir `TextColor="White"` hardcoded por `{StaticResource White}`

### 🟡 Aviso
- [ ] Adicionar `AutomationId` em:
  - `Entry` Título → `AutomationId="TitleEntry"`
  - `Picker` Categoria → `AutomationId="CategoryPicker"`
  - `Picker` Status → `AutomationId="StatusPicker"`
  - `Entry` Pontuação → `AutomationId="ScoreEntry"`
  - `Switch` Script Sequencial → `AutomationId="SequentialScriptSwitch"`
  - `Button` Salvar → `AutomationId="SaveButton"`
  - `Button` Excluir → `AutomationId="DeleteButton"`
  - `Button` Cancelar → `AutomationId="CancelButton"`
- [ ] Adicionar `SemanticProperties.Description` nos botões de ação

### 🔵 Sugestão
- [ ] Substituir `StackLayout` por `VerticalStackLayout` nas seções internas
- [ ] Criar `Style` local `x:Key="CardSection"` para as seções `BackgroundColor="White" Padding="15"` (reutilizado em 4 locais)
- [ ] Adicionar `SemanticProperties.HeadingLevel="Level2"` nos títulos de seção

---

## PASSO 5 — CameraPage.xaml 🔴🟡🔵

**Arquivo:** `CameraApp/Views/CameraPage.xaml`

### 🔴 Crítico
- [ ] Substituir `BackgroundColor="LightGray"` por `{StaticResource Gray200}` (2 `Border`s)
- [ ] Substituir `BackgroundColor="Red"` no botão Limpar por `{StaticResource Danger}`
- [ ] Substituir `TextColor="Gray"` no label de placeholder por `{StaticResource Gray500}`
- [ ] Substituir `TextColor="White"` hardcoded nos botões por `{StaticResource White}`
- [ ] Adicionar estado de loading: `ActivityIndicator IsVisible="{Binding IsLoading}"` e label de erro

### 🟡 Aviso
- [ ] Adicionar `SemanticProperties.Description` na `Image` de foto
- [ ] Adicionar `SemanticProperties.Description` nos botões
- [ ] Adicionar `AutomationId` em:
  - `Button` Tirar Foto → `AutomationId="TakePhotoButton"`
  - `Button` Galeria → `AutomationId="PickPhotoButton"`
  - `Button` Limpar → `AutomationId="ClearPhotoButton"`
  - `Image` foto → `AutomationId="PhotoImage"`
- [ ] Remover `WidthRequest="300"` fixo da `Image` — usar `HorizontalOptions="Fill"` para responsividade

### 🔵 Sugestão
- [ ] Substituir `StackLayout` por `VerticalStackLayout`

---

## PASSO 6 — MapPage.xaml 🔴🟡

**Arquivo:** `CameraApp/Views/MapPage.xaml`

### 🔴 Crítico
- [ ] Substituir `BackgroundColor="LightGray"` no `WebView` por `{StaticResource Gray200}`
- [ ] Substituir `TextColor="White"` hardcoded por `{StaticResource White}` (múltiplas ocorrências no header)

### 🟡 Aviso
- [ ] Localizar `Title="Mapa - Localização GPS"` usando `LocalizationResourceManager`
- [ ] Adicionar `SemanticProperties.Description` nos botões
- [ ] Adicionar `AutomationId` em:
  - `Button` Atualizar Localização → `AutomationId="UpdateLocationButton"`
  - `Button` Resetar Zoom → `AutomationId="ResetMapButton"`
  - `WebView` → `AutomationId="MapWebView"`

---

## PASSO 7 — PosturePage.xaml 🔴🟡🔵

**Arquivo:** `CameraApp/Views/PosturePage.xaml`

### 🔴 Crítico
- [ ] Substituir cores de seção hardcoded por tokens:
  - `LightGray` → `{StaticResource Gray100}`
  - `LightBlue` → `{StaticResource Secondary}` com opacity ou novo token
  - `LightYellow` → `{StaticResource Gray100}`
  - `LightPink` → `{StaticResource Gray100}`
  - `LightGreen` → `{StaticResource Gray100}`
- [ ] Substituir `BackgroundColor="Green"` (botão Start) por `{StaticResource Success}`
- [ ] Substituir `BackgroundColor="Red"` (botão Stop) por `{StaticResource Danger}`
- [ ] Substituir `BackgroundColor="Gray"` (botão Reset) por `{StaticResource Gray500}`
- [ ] Substituir `ThumbColor="Blue"` por `{StaticResource Primary}`
- [ ] Substituir `ThumbColor="Orange"` por `{StaticResource Tertiary}` (ou novo token `Warning`)
- [ ] Substituir `TextColor="Gray"` (2×) por `{StaticResource Gray500}`
- [ ] Localizar `Title="Monitor de Postura"` usando `LocalizationResourceManager`

### 🟡 Aviso
- [ ] Adicionar `SemanticProperties.Description` nos `Slider`s e botões
- [ ] Adicionar `AutomationId` em:
  - `Button` Iniciar → `AutomationId="StartMonitoringButton"`
  - `Button` Parar → `AutomationId="StopMonitoringButton"`
  - `Slider` Sensibilidade → `AutomationId="SensitivitySlider"`
  - `Slider` AlertDelay → `AutomationId="AlertDelaySlider"`
  - `Button` Resetar Stats → `AutomationId="ResetStatsButton"`
- [ ] Padronizar todas as seções com `{StaticResource Gray100}` para consistência visual

### 🔵 Sugestão
- [ ] Substituir `StackLayout` por `VerticalStackLayout`
- [ ] Criar `Style` local `x:Key="PostureSection"` para reutilizar nas 5 seções

---

## PASSO 8 — AdvancedFiltersPage.xaml 🟡

**Arquivo:** `CameraApp/Views/AdvancedFiltersPage.xaml`

### 🟡 Aviso
- [ ] Adicionar `SemanticProperties.Description` no `CheckBox` de Script Sequencial e Ordenação Crescente
- [ ] Adicionar `AutomationId` em:
  - `Picker` Categoria → `AutomationId="CategoryFilterPicker"`
  - `Picker` Status → `AutomationId="StatusFilterPicker"`
  - `DatePicker` início → `AutomationId="StartDatePicker"`
  - `DatePicker` fim → `AutomationId="EndDatePicker"`
  - `Entry` Criado Por → `AutomationId="CreatedByEntry"`
  - `Entry` Min Score → `AutomationId="MinScoreEntry"`
  - `Entry` Max Score → `AutomationId="MaxScoreEntry"`
  - `Button` Limpar Tudo → `AutomationId="ClearFiltersButton"`
  - `Button` Aplicar → `AutomationId="ApplyFiltersButton"`
- [ ] Localizar strings hardcoded do `Picker` de ordenação (`Título`, `Data de Criação`, etc.) usando `.resx`

---

## PASSO 9 — Validação de Build 🔴

Após todas as alterações, executar:

```powershell
dotnet build -f net10.0-android
```

Verificar:
- [ ] Zero erros de compilação
- [ ] Nenhum `StaticResource` com chave faltante
- [ ] Nenhuma referência a `DynamicResource` sem token definido

---

## Resumo de Esforço Estimado

| Passo | Escopo | Impacto |
|---|---|---|
| 1 — Colors.xaml | 1 arquivo, 4 linhas | Desbloqueia Passos 2, 5, 7 |
| 2 — LoginPage | 3 críticos + 3 avisos | Alto — tela de entrada do app |
| 3 — FormListPage | 2 críticos + 3 avisos | Alto — tela principal |
| 4 — FormEditPage | 3 críticos + 2 avisos | Alto — operações de CRUD |
| 5 — CameraPage | 4 críticos + 3 avisos | Médio |
| 6 — MapPage | 2 críticos + 3 avisos | Médio |
| 7 — PosturePage | 4 críticos + 3 avisos | Médio — maior volume de hardcode |
| 8 — AdvancedFiltersPage | 3 avisos | Baixo — já a mais limpa |
| 9 — Build | Validação | Obrigatório |

**Total de itens críticos: 18 | Total de avisos: 23 | Total de sugestões: 11**
