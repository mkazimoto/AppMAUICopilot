# Plano de Execução — Especialista em Localização

> **Projeto:** CameraApp .NET MAUI  
> **Data:** 13/03/2026  
> **Escopo:** Auditoria completa do projeto  
> **Idiomas:** pt (base) · en · es

---

## 📊 Resumo do Estado Atual

| Idioma | Total de chaves | Faltando | Paridade |
|--------|----------------|---------|---------|
| pt (base) | 105 | — | ✅ |
| en | 105 | 0 | ✅ |
| es | 105 | 0 | ✅ |

**Paridade de chaves entre idiomas:** completa — nenhuma chave ausente entre pt/en/es.

---

## 🔴 Fase 1 — Crítico (bloquear entrega)

### Tarefa 1.1 — Chaves referenciadas no código mas ausentes em todos os `.resx` [L-02]

**Arquivo:** `ViewModels/CameraPageViewModel.cs`

Três chaves são referenciadas via `LocalizationResourceManager.Instance[...]` mas **não existem em nenhum arquivo `.resx`**:

| Chave | Linhas | Fallback atual |
|-------|--------|---------------|
| `Camera_CaptureError` | CameraPageViewModel.cs:45 | `"Error capturing photo"` |
| `Camera_UnexpectedError` | CameraPageViewModel.cs:49, 72 | `"Unexpected error"` |
| `Camera_PickError` | CameraPageViewModel.cs:68 | `"Error selecting photo"` |

**Ação:** Adicionar as 3 chaves nos 3 arquivos `.resx` com tradução real.

```xml
<!-- Câmera — mensagens de erro (adicionar em AppResources.resx) -->
<data name="Camera_CaptureError" xml:space="preserve">
  <value>Erro ao capturar foto</value>
</data>
<data name="Camera_UnexpectedError" xml:space="preserve">
  <value>Erro inesperado</value>
</data>
<data name="Camera_PickError" xml:space="preserve">
  <value>Erro ao selecionar foto</value>
</data>
```

```xml
<!-- em AppResources.en.resx -->
<data name="Camera_CaptureError" xml:space="preserve">
  <value>Error capturing photo</value>
</data>
<data name="Camera_UnexpectedError" xml:space="preserve">
  <value>Unexpected error</value>
</data>
<data name="Camera_PickError" xml:space="preserve">
  <value>Error selecting photo</value>
</data>
```

```xml
<!-- em AppResources.es.resx -->
<data name="Camera_CaptureError" xml:space="preserve">
  <value>Error al capturar foto</value>
</data>
<data name="Camera_UnexpectedError" xml:space="preserve">
  <value>Error inesperado</value>
</data>
<data name="Camera_PickError" xml:space="preserve">
  <value>Error al seleccionar foto</value>
</data>
```

---

### Tarefa 1.2 — Strings hardcoded em XAML [L-03]

Strings visíveis ao usuário escritas diretamente em atributos XAML sem uso de binding de localização:

| Arquivo | Linha | Atributo | Valor atual | Chave sugerida |
|---------|-------|----------|-------------|---------------|
| `LoginPage.xaml` | 9 | `Title=` | `"TOTVS RM - Login"` | `AppTitle` (já existe) |
| `LoginPage.xaml` | 95 | `Placeholder=` | `"Ex: CorporeRM"` | `Login_ServiceAliasPlaceholder` (nova) |
| `FormEditPage.xaml` | 165 | `Text=` | `"ID:"` | `FormEdit_IdLabel` (nova) |

**Ação para `LoginPage.xaml:9`:** Substituir pelo binding existente:
```xml
<!-- Antes -->
Title="TOTVS RM - Login"

<!-- Depois -->
Title="{Binding [AppTitle], Source={x:Static services:LocalizationResourceManager.Instance}}"
```

**Ação para `LoginPage.xaml:95`:** Criar nova chave `Login_ServiceAliasPlaceholder` e aplicar binding:
```xml
<!-- Antes -->
Placeholder="Ex: CorporeRM"

<!-- Depois -->
Placeholder="{Binding [Login_ServiceAliasPlaceholder], Source={x:Static services:LocalizationResourceManager.Instance}}"
```

Tradução da nova chave:
| Chave | pt | en | es |
|-------|----|----|-----|
| `Login_ServiceAliasPlaceholder` | `Ex: CorporeRM` | `e.g.: CorporeRM` | `Ej: CorporeRM` |

**Ação para `FormEditPage.xaml:165`:** Criar nova chave `FormEdit_IdLabel`:
```xml
<!-- Antes -->
Text="ID:"

<!-- Depois -->
Text="{Binding [FormEdit_IdLabel], Source={x:Static services:LocalizationResourceManager.Instance}}"
```

Tradução da nova chave:
| Chave | pt | en | es |
|-------|----|----|-----|
| `FormEdit_IdLabel` | `ID:` | `ID:` | `ID:` |

> **Nota:** As entradas `Placeholder="0"`, `Placeholder="100"` e `Text="X:"/"Y:"/"Z:"` são valores técnicos/numéricos e **não precisam** ser localizados.

---

### Tarefa 1.3 — DisplayAlert com strings hardcoded em C# [L-04]

Todos os ViewModels chamam `DisplayAlertAsync`/`DisplayAlert` com strings literais em português. Nenhum usa `LocalizationResourceManager.Instance`.

| Arquivo | Linha | Strings hardcoded |
|---------|-------|-------------------|
| `LoginViewModel.cs` | 115 | `"Confirmação"`, `"Deseja realmente sair?"`, `"Sair"`, `"Cancelar"` |
| `FormListViewModel.cs` | 389 | `"Sessão Expirada"`, `"Sua sessão expirou..."` |
| `FormListViewModel.cs` | 414 | `"Erro"` |
| `FormEditViewModel.cs` | 190 | `"Confirmar"`, `"Tem certeza que deseja excluir este formulário?"`, `"Sim"`, `"Não"` |
| `FormEditViewModel.cs` | ~220 | `"Sucesso"`, `"Formulário excluído com sucesso!"`, `"Erro"`, `"Erro ao excluir formulário."` |
| `MapPageViewModel.cs` | 69 | `"Erro"`, `"Não foi possível obter sua localização..."` |
| `MapPageViewModel.cs` | 78 | `"Erro"`, `"Erro ao obter localização..."` |
| `PosturePageViewModel.cs` | 102 | `"Erro"`, `"Não foi possível iniciar o monitoramento..."` |

**Novas chaves necessárias:**

| Chave | pt | en | es |
|-------|----|----|-----|
| `Common_Confirm` | `Confirmação` | `Confirmation` | `Confirmación` |
| `Common_SessionExpired` | `Sessão Expirada` | `Session Expired` | `Sesión Expirada` |
| `Common_SessionExpiredMessage` | `Sua sessão expirou. Você será redirecionado para fazer login novamente.` | `Your session has expired. You will be redirected to log in again.` | `Su sesión ha expirado. Será redirigido para iniciar sesión nuevamente.` |
| `Login_LogoutConfirmMessage` | `Deseja realmente sair?` | `Are you sure you want to sign out?` | `¿Realmente desea salir?` |
| `FormEdit_DeleteConfirmMessage` | `Tem certeza que deseja excluir este formulário?` | `Are you sure you want to delete this form?` | `¿Está seguro de que desea eliminar este formulario?` |
| `FormEdit_DeleteSuccess` | `Formulário excluído com sucesso!` | `Form deleted successfully!` | `¡Formulario eliminado con éxito!` |
| `FormEdit_DeleteError` | `Erro ao excluir formulário.` | `Error deleting form.` | `Error al eliminar el formulario.` |
| `Map_LocationError` | `Não foi possível obter sua localização. Verifique se as permissões estão habilitadas e se o GPS está ativo.` | `Could not get your location. Please check that permissions are enabled and GPS is active.` | `No se pudo obtener su ubicación. Verifique que los permisos estén habilitados y el GPS esté activo.` |
| `Map_LocationFetchError` | `Erro ao obter localização:` | `Error getting location:` | `Error al obtener la ubicación:` |
| `Posture_StartError` | `Não foi possível iniciar o monitoramento:` | `Could not start monitoring:` | `No se pudo iniciar el monitoreo:` |

**Padrão de substituição nos ViewModels:**
```csharp
// Antes
await Shell.Current?.DisplayAlertAsync("Confirmação", "Deseja realmente sair?", "Sair", "Cancelar");

// Depois
var loc = LocalizationResourceManager.Instance;
await Shell.Current?.DisplayAlertAsync(
    loc["Common_Confirm"].ToString()!,
    loc["Login_LogoutConfirmMessage"].ToString()!,
    loc["Login_SignOut"].ToString()!,
    loc["Common_Cancel"].ToString()!);
```

---

## 🟡 Fase 2 — Aviso (corrigir antes do próximo sprint)

### Tarefa 2.1 — SemanticProperties.Description hardcoded em XAML [L-03 estendido]

Todas as descrições de acessibilidade (`SemanticProperties.Description`) estão em português fixo. Isso interrompe a acessibilidade em usuários que escolhem `en` ou `es`.

**Ocorrências:**

| Arquivo | Linha | Valor atual |
|---------|-------|-------------|
| `AdvancedFiltersPage.xaml` | 129 | `"Filtrar apenas scripts sequenciais"` |
| `AdvancedFiltersPage.xaml` | 212 | `"Ordenar resultados em ordem crescente"` |
| `AdvancedFiltersPage.xaml` | 234 | `"Limpar todos os filtros"` |
| `AdvancedFiltersPage.xaml` | 244 | `"Aplicar filtros selecionados"` |
| `CameraPage.xaml` | 28 | `"Tirar foto com a câmera"` |
| `CameraPage.xaml` | 37 | `"Selecionar foto da galeria"` |
| `CameraPage.xaml` | 57 | `"Foto selecionada"` |
| `CameraPage.xaml` | 65 | `"Remover foto selecionada"` |
| `FormListPage.xaml` | 70 | `"Buscar formulários"` |
| `FormListPage.xaml` | 149 | `"Abrir formulário para edição"` |
| `PosturePage.xaml` | 45 | `"Iniciar monitoramento de postura"` |
| `PosturePage.xaml` | 54 | `"Parar monitoramento de postura"` |
| `PosturePage.xaml` | 103 | `"Ajustar sensibilidade do detector de postura"` |
| `PosturePage.xaml` | 118 | `"Ajustar tempo de atraso para alerta de postura"` |
| `PosturePage.xaml` | 146 | `"Resetar estatísticas de alertas"` |
| `FormEditPage.xaml` | 187 | `"Salvar formulário"` |
| `FormEditPage.xaml` | 203 | `"Excluir formulário"` |
| `FormEditPage.xaml` | 218 | `"Cancelar edição"` |
| `MapPage.xaml` | 66 | `"Atualizar localização GPS"` |
| `MapPage.xaml` | 74 | `"Resetar zoom do mapa"` |
| `LoginPage.xaml` | 112 | `"Limpar credenciais"` |

**Ação:** Criar chaves `*_Hint` ou `*_Description` e substituir com binding.

**Novas chaves sugeridas (prefixo `Accessibility_`):**

| Chave | pt | en | es |
|-------|----|----|-----|
| `Accessibility_FilterSequential` | `Filtrar apenas scripts sequenciais` | `Filter sequential scripts only` | `Filtrar solo scripts secuenciales` |
| `Accessibility_SortAscending` | `Ordenar resultados em ordem crescente` | `Sort results in ascending order` | `Ordenar resultados en orden ascendente` |
| `Accessibility_ClearFilters` | `Limpar todos os filtros` | `Clear all filters` | `Limpiar todos los filtros` |
| `Accessibility_ApplyFilters` | `Aplicar filtros selecionados` | `Apply selected filters` | `Aplicar filtros seleccionados` |
| `Accessibility_TakePhoto` | `Tirar foto com a câmera` | `Take a photo with the camera` | `Tomar foto con la cámara` |
| `Accessibility_SelectFromGallery` | `Selecionar foto da galeria` | `Select a photo from gallery` | `Seleccionar foto de la galería` |
| `Accessibility_SelectedPhoto` | `Foto selecionada` | `Selected photo` | `Foto seleccionada` |
| `Accessibility_RemovePhoto` | `Remover foto selecionada` | `Remove selected photo` | `Eliminar foto seleccionada` |
| `Accessibility_SearchForms` | `Buscar formulários` | `Search forms` | `Buscar formularios` |
| `Accessibility_OpenFormEdit` | `Abrir formulário para edição` | `Open form for editing` | `Abrir formulario para edición` |
| `Accessibility_StartPostureMonitor` | `Iniciar monitoramento de postura` | `Start posture monitoring` | `Iniciar monitoreo de postura` |
| `Accessibility_StopPostureMonitor` | `Parar monitoramento de postura` | `Stop posture monitoring` | `Detener monitoreo de postura` |
| `Accessibility_AdjustSensitivity` | `Ajustar sensibilidade do detector de postura` | `Adjust posture detector sensitivity` | `Ajustar sensibilidad del detector de postura` |
| `Accessibility_AdjustAlertDelay` | `Ajustar tempo de atraso para alerta de postura` | `Adjust posture alert delay time` | `Ajustar tiempo de retraso para alerta de postura` |
| `Accessibility_ResetAlertStats` | `Resetar estatísticas de alertas` | `Reset alert statistics` | `Restablecer estadísticas de alertas` |
| `Accessibility_SaveForm` | `Salvar formulário` | `Save form` | `Guardar formulario` |
| `Accessibility_DeleteForm` | `Excluir formulário` | `Delete form` | `Eliminar formulario` |
| `Accessibility_CancelEdit` | `Cancelar edição` | `Cancel edit` | `Cancelar edición` |
| `Accessibility_UpdateLocation` | `Atualizar localização GPS` | `Update GPS location` | `Actualizar ubicación GPS` |
| `Accessibility_ResetMapZoom` | `Resetar zoom do mapa` | `Reset map zoom` | `Restablecer zoom del mapa` |
| `Accessibility_ClearCredentials` | `Limpar credenciais` | `Clear credentials` | `Limpiar credenciales` |

**Padrão de substituição no XAML:**
```xml
<!-- Antes -->
SemanticProperties.Description="Tirar foto com a câmera"

<!-- Depois -->
SemanticProperties.Description="{Binding [Accessibility_TakePhoto], Source={x:Static services:LocalizationResourceManager.Instance}}"
```

---

### Tarefa 2.2 — Chaves duplicadas candidatas a `Common_*` [L-07]

As chaves abaixo possuem valores idênticos aos já definidos em `Common_*` e podem ser removidas em favor das comuns:

| Chave redundante | Chave `Common_*` equivalente | Impacto |
|-----------------|------------------------------|---------|
| `FormEdit_Cancel` | `Common_Cancel` | Substituir binding em `FormEditPage.xaml` |
| `FormEdit_Save` | `Common_Save` | Substituir binding em `FormEditPage.xaml` |
| `FormEdit_Delete` | `Common_Delete` | Substituir binding em `FormEditPage.xaml` |

> ⚠️ Antes de remover qualquer chave, verificar com `grep_search` se há outras referências espalhadas pelo projeto.

---

## 🔵 Fase 3 — Sugestões de qualidade

### Tarefa 3.1 — Adicionar comentários `<comment>` em chaves não óbvias [L-09]

| Chave | Comentário sugerido |
|-------|---------------------|
| `FormList_By` | Usado no padrão "Criado em: X por Y" |
| `Filter_Until` | Separador "de … até" no filtro de período (valor curto: "até") |
| `Filter_Score` | Seção de pontuação mínima/máxima nos filtros avançados |
| `Login_Authenticating` | Texto de loading mostrado durante o processo de autenticação |

### Tarefa 3.2 — Agrupar chaves por tela com comentários XML [L-10]

Os arquivos `.en.resx` e `.es.resx` já possuem agrupamento por comentários. Verificar consistência nos comentários de seção entre os 3 arquivos.

### Tarefa 3.3 — Mensagens de erro de `ApiException` para `.resx` [L-11]

Em `FormListViewModel.cs:414`, a mensagem de erro da API é montada dinamicamente com strings literais. Avaliar extração de templates para as chaves:
- `Common_ApiError` — `"Erro da API ({0})"` 
- `Common_ApiErrorHintFE018` — `"\n\nDica: Verifique se os filtros estão corretos ou tente limpar os filtros."`

---

## 📋 Ordem de Execução Recomendada

```
Fase 1 — Crítico
  ├── [1.1] Adicionar 3 chaves Camera_* faltantes nos 3 .resx      ← ~10 min
  ├── [1.2] Corrigir 3 strings hardcoded no XAML (Title, Placeholder, ID:)  ← ~15 min
  └── [1.3] Substituir 10+ DisplayAlert com strings hardcoded       ← ~45 min
        ├── Criar 9 novas chaves nos 3 .resx
        └── Substituir cada chamada nos ViewModels

Fase 2 — Aviso
  ├── [2.1] Localizar 21 SemanticProperties.Description             ← ~60 min
  │         ├── Criar 21 chaves Accessibility_* nos 3 .resx
  │         └── Substituir cada ocorrência no XAML
  └── [2.2] Consolidar FormEdit_Cancel/Save/Delete → Common_*        ← ~15 min

Fase 3 — Sugestões
  ├── [3.1] Adicionar <comment> em 4 chaves                         ← ~5 min
  ├── [3.2] Revisar agrupamento nos 3 .resx                        ← ~10 min
  └── [3.3] Avaliar extração de mensagens de ApiException           ← ~20 min
```

---

## ✅ Checklist de Validação Pós-Execução

- [x] `Camera_CaptureError`, `Camera_UnexpectedError`, `Camera_PickError` presentes nos 3 `.resx`
- [x] `LoginPage.xaml` — `Title=` usa binding de localização
- [x] `FormEditPage.xaml` — `Text="ID:"` usa binding de localização
- [x] Todos os `DisplayAlertAsync` em ViewModels usam `LocalizationResourceManager.Instance`
- [x] Todos os `SemanticProperties.Description` usam binding de localização
- [x] Nenhuma nova chave tem valor vazio em qualquer idioma
- [x] Build passa sem erros: `dotnet build -f net10.0-android`
- [x] Paridade de chaves entre pt/en/es verificada após alterações

---

## 📁 Arquivos Impactados

| Arquivo | Fase | Tipo de mudança |
|---------|------|----------------|
| `Resources/Strings/AppResources.resx` | 1, 2, 3 | Adicionar chaves |
| `Resources/Strings/AppResources.en.resx` | 1, 2, 3 | Adicionar chaves |
| `Resources/Strings/AppResources.es.resx` | 1, 2, 3 | Adicionar chaves |
| `Views/LoginPage.xaml` | 1.2, 2.1 | Substituir hardcodes |
| `Views/FormEditPage.xaml` | 1.2, 2.1, 2.2 | Substituir hardcodes |
| `Views/AdvancedFiltersPage.xaml` | 2.1 | Substituir SemanticProperties |
| `Views/CameraPage.xaml` | 2.1 | Substituir SemanticProperties |
| `Views/FormListPage.xaml` | 2.1 | Substituir SemanticProperties |
| `Views/PosturePage.xaml` | 2.1 | Substituir SemanticProperties |
| `Views/MapPage.xaml` | 2.1 | Substituir SemanticProperties |
| `ViewModels/LoginViewModel.cs` | 1.3 | Substituir DisplayAlert |
| `ViewModels/FormListViewModel.cs` | 1.3 | Substituir DisplayAlert |
| `ViewModels/FormEditViewModel.cs` | 1.3 | Substituir DisplayAlert |
| `ViewModels/MapPageViewModel.cs` | 1.3 | Substituir DisplayAlert |
| `ViewModels/PosturePageViewModel.cs` | 1.3 | Substituir DisplayAlert |
