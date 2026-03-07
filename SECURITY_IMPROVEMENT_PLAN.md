# Plano de Melhoria de Segurança — CameraApp .NET MAUI

> **Data da auditoria:** 07/03/2026  
> **Versão do projeto:** .NET 10 / MAUI 10  
> **Referência:** OWASP Top 10, OWASP Mobile Top 10

---

## Sumário Executivo

A auditoria identificou **4 vulnerabilidades críticas**, **6 de média severidade** e **5 sugestões de hardening**. Os problemas críticos envolvem comunicação sem TLS, URL de ambiente hardcoded, condição de corrida no refresh de token e configuração incorreta do manifesto Android. Nenhuma credencial ou chave de API foi encontrada hardcoded no código-fonte, e o armazenamento de tokens em `SecureStorage` está correto.

---

## Resumo Priorizado

| ID     | Severidade      | Arquivo                                                              | Ação resumida                              |
|--------|-----------------|----------------------------------------------------------------------|--------------------------------------------|
| SEC-01 | 🔴 Crítico      | `CameraApp/Config/ApiConfig.cs`                                      | Trocar `http://` por `https://`            |
| SEC-02 | 🔴 Crítico      | `CameraApp/Config/ApiConfig.cs`                                      | Externalizar URL de produção               |
| SEC-03 | 🔴 Crítico      | `CameraApp/Services/AuthHttpHandler.cs`                              | Implementar `SemaphoreSlim` no refresh     |
| SEC-04 | 🔴 Crítico      | `CameraApp/Platforms/Android/AndroidManifest.xml`                    | Remover `usesCleartextTraffic="true"`      |
| SEC-05 | 🟡 Aviso        | `CameraApp/ViewModels/LoginViewModel.cs`                             | Validação de entrada no login              |
| SEC-06 | 🟡 Aviso        | `CameraApp/Services/BaseService.cs`, `AuthService.cs`                | `JsonSerializerOptions` com limites        |
| SEC-07 | 🟡 Aviso        | `CameraApp/Platforms/Android/AndroidManifest.xml`                    | `allowBackup="false"`                      |
| SEC-08 | 🟡 Aviso        | `CameraApp/Services/AuthService.cs`                                  | Não logar mensagem de erro da API          |
| SEC-09 | 🟡 Aviso        | Múltiplos services/viewmodels                                        | Tratar exceções específicas                |
| SEC-10 | 🟡 Aviso        | `CameraApp/Services/AuthService.cs`                                  | Adicionar `CancellationToken` ao login     |
| SEC-11 | 🔵 Sugestão     | `CameraApp/CameraApp.csproj`                                         | Habilitar R8/ProGuard na release           |
| SEC-12 | 🔵 Sugestão     | `CameraApp/MauiProgram.cs`                                           | Certificate pinning                        |
| SEC-13 | 🔵 Sugestão     | `CameraApp/MauiProgram.cs`                                           | Timeout e `MaxResponseContentBufferSize`   |
| SEC-14 | 🔵 Sugestão     | `CameraApp/Services/BaseService.cs`                                  | Verificação explícita de `IsSuccessStatusCode` |
| SEC-15 | 🔵 Sugestão     | `CameraApp/Platforms/iOS/Info.plist`                                 | Verificar `NSAllowsArbitraryLoads`         |

---

## 🔴 Vulnerabilidades Críticas

---

### [SEC-01] Comunicação sem TLS — `http://` hardcoded

**Arquivo:** `CameraApp/Config/ApiConfig.cs`  
**OWASP:** Mobile M5 — Insecure Communication / A02 — Cryptographic Failures

**Problema:** Toda a comunicação entre o app e a API trafega sem criptografia. Credenciais de login, tokens JWT e dados de formulários ficam expostos a ataques man-in-the-middle (MITM).

```csharp
// ❌ Atual — sem criptografia
public const string BaseUrl = "http://10.0.2.2:5000";
```

**Correção:**
```csharp
// ✅ Produção sempre HTTPS
public static class ApiConfig
{
#if DEBUG
    public const string BaseUrl = "https://10.0.2.2:5001"; // HTTPS mesmo em dev
#else
    public const string BaseUrl = "https://api.suaempresa.com";
#endif
}
```

**Esforço:** Baixo | **Impacto:** Alto

---

### [SEC-02] IP de emulador hardcoded no código-fonte

**Arquivo:** `CameraApp/Config/ApiConfig.cs`  
**OWASP:** Mobile M2 — Inadequate Supply Chain Security

**Problema:** `10.0.2.2` é o loopback do emulador Android. Se esse valor vazar para produção, o aplicativo apontaria para o ambiente errado ou exporia a URL de produção no código compilado.

```csharp
// ❌ Atual — IP de desenvolvimento no código-fonte
public const string BaseUrl = "http://10.0.2.2:5000";
```

**Correção:** Externalizar para arquivo de configuração excluído do controle de versão ou usar `Build Configurations`:
```csharp
// ✅ Separar URLs por ambiente
#if DEBUG
    public const string BaseUrl = "https://10.0.2.2:5001";
#else
    public const string BaseUrl = "https://api.suaempresa.com";
#endif
```

Para projetos maiores, considerar `appsettings.json` com transformações por ambiente (excluídas do `.gitignore` para valores de produção).

**Esforço:** Baixo | **Impacto:** Médio

---

### [SEC-03] Race condition no refresh de token

**Arquivo:** `CameraApp/Services/AuthHttpHandler.cs`  
**OWASP:** A07 — Identification and Authentication Failures

**Problema:** A flag `_isRefreshing` é um `bool` simples, sem garantia de atomicidade em leituras/escritas concorrentes. Com múltiplas requisições paralelas retornando 401 simultaneamente, o check `!_isRefreshing` pode ser avaliado `true` por várias threads antes que qualquer uma sete o flag. Resultado: múltiplos refresh tokens enviados à API ao mesmo tempo, podendo invalidar o token vigente e forçar logout indesejado.

```csharp
// ❌ Atual — não thread-safe
private bool _isRefreshing = false;

if (response.StatusCode == HttpStatusCode.Unauthorized && !_isRefreshing)
{
    _isRefreshing = true;
    // múltiplas threads passam aqui antes que _isRefreshing seja setada
}
```

**Correção — usar `SemaphoreSlim`:**
```csharp
// ✅ Thread-safe com SemaphoreSlim
private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
private bool _isRefreshing = false;

protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request, CancellationToken cancellationToken)
{
    var token = await _authService.GetTokenAsync();
    if (token != null)
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

    var response = await base.SendAsync(request, cancellationToken);

    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (_isRefreshing) return response;
            _isRefreshing = true;

            var refreshed = await _authService.RefreshTokenAsync();
            if (refreshed)
            {
                var newToken = await _authService.GetTokenAsync();
                if (newToken != null)
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", newToken.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            else
            {
                await _authService.LogoutAsync();
            }
        }
        finally
        {
            _isRefreshing = false;
            _refreshLock.Release();
        }
    }

    return response;
}
```

**Esforço:** Médio | **Impacto:** Alto

---

### [SEC-04] Cleartext traffic permitido no Android

**Arquivo:** `CameraApp/Platforms/Android/AndroidManifest.xml`  
**OWASP:** Mobile M5 — Insecure Communication

**Problema:** `android:usesCleartextTraffic="true"` permite que o app faça conexões HTTP abertas em dispositivos Android 9+ (API 28+), que por padrão bloqueiam cleartext. Combinado com SEC-01, o app funciona plenamente sem TLS.

```xml
<!-- ❌ Atual -->
<application android:usesCleartextTraffic="true" ...>
```

**Correção:**
```xml
<!-- ✅ AndroidManifest.xml — referenciar network security config -->
<application
    android:networkSecurityConfig="@xml/network_security_config"
    android:usesCleartextTraffic="false"
    ...>
```

```xml
<!-- ✅ Platforms/Android/Resources/xml/network_security_config.xml -->
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <base-config cleartextTrafficPermitted="false">
        <trust-anchors>
            <certificates src="system" />
        </trust-anchors>
    </base-config>
</network-security-config>
```

**Esforço:** Baixo | **Impacto:** Alto

---

## 🟡 Avisos (Média Severidade)

---

### [SEC-05] Ausência de validação de entrada no `LoginViewModel`

**Arquivo:** `CameraApp/ViewModels/LoginViewModel.cs`  
**OWASP:** A03 — Injection / A05 — Security Misconfiguration

**Problema:** Sem validação de `Username` e `Password` antes do envio, o app pode enviar credenciais vazias ou excessivamente longas, expor mensagens de erro detalhadas da API ao usuário e causar erros desnecessários no backend.

**Correção:**
```csharp
[RelayCommand]
private async Task LoginAsync()
{
    if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
    {
        ErrorMessage = AppResources.ValidationRequiredFields;
        return;
    }
    if (Username.Length > 100 || Password.Length > 128)
    {
        ErrorMessage = AppResources.ValidationFieldsTooLong;
        return;
    }
    // prosseguir com o login...
}
```

**Esforço:** Baixo | **Impacto:** Médio

---

### [SEC-06] Desserialização JSON sem limites de tamanho

**Arquivos:** `CameraApp/Services/AuthService.cs`, `CameraApp/Services/BaseService.cs`, `CameraApp/Services/FormServiceGeneric.cs`  
**OWASP:** A05 — Security Misconfiguration

**Problema:** Sem `JsonSerializerOptions` com limites, uma resposta maliciosa ou corrompida de tamanho arbitrário pode causar consumo excessivo de memória no cliente.

**Correção:**
```csharp
// ✅ Definir opções compartilhadas com MaxDepth
private static readonly JsonSerializerOptions _jsonOptions = new()
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    MaxDepth = 32 // padrão é 64, reduzir para limitar payloads aninhados
};

// ✅ MauiProgram.cs — limitar buffer de resposta no HttpClient
services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.MaxResponseContentBufferSize = 5 * 1024 * 1024; // 5 MB
});
```

**Esforço:** Baixo | **Impacto:** Médio

---

### [SEC-07] `android:allowBackup="true"` expõe dados do app

**Arquivo:** `CameraApp/Platforms/Android/AndroidManifest.xml`  
**OWASP:** Mobile M9 — Insecure Data Storage

**Problema:** `android:allowBackup="true"` permite que o ADB extraia o backup completo do app, incluindo o keystore local onde o `SecureStorage` persiste tokens. Um atacante com acesso físico ao dispositivo em modo desenvolvedor pode extrair tokens de autenticação.

**Correção:**
```xml
<!-- ✅ Desabilitar backup para proteger dados sensíveis -->
<application
    android:allowBackup="false"
    android:fullBackupContent="false"
    ...>
```

Se backup de dados não sensíveis for necessário, usar um `backup_rules.xml` exclusivo:
```xml
<!-- res/xml/backup_rules.xml -->
<?xml version="1.0" encoding="utf-8"?>
<full-backup-content>
    <exclude domain="sharedpref" path="." />
    <exclude domain="database" path="." />
</full-backup-content>
```

**Esforço:** Baixo | **Impacto:** Médio

---

### [SEC-08] Logging de mensagens de erro da API

**Arquivo:** `CameraApp/Services/AuthService.cs`  
**OWASP:** A09 — Security Logging and Monitoring Failures

**Problema:** A mensagem de erro retornada pela API durante falhas de autenticação pode conter informações sensíveis (stack trace, detalhes de banco de dados). Logar o corpo da resposta expõe essas informações nos logs do dispositivo.

```csharp
// ❌ Atual — pode incluir dados sensíveis do backend
_logger.LogWarning("Login failed: {StatusCode} - {Error}", response.StatusCode, error?.Message);
```

**Correção:**
```csharp
// ✅ Logar apenas o status code em endpoints de autenticação
_logger.LogWarning("Login failed with status: {StatusCode}", response.StatusCode);
// Nunca logar error?.Message de endpoints de autenticação em produção
```

**Esforço:** Baixo | **Impacto:** Médio

---

### [SEC-09] Tratamento genérico de exceções silencia falhas de segurança

**Arquivos:** `CameraApp/Services/AuthService.cs`, `CameraApp/Services/LocationService.cs`, `CameraApp/ViewModels/LoginViewModel.cs`  
**OWASP:** A09 — Security Logging and Monitoring Failures

**Problema:** `catch (Exception)` captura inclusive `OperationCanceledException`, `TaskCanceledException` e `JsonException`, silenciando tipos específicos de falha que têm implicações de segurança diferentes.

```csharp
// ❌ Atual — mascara tipo real do erro
catch (Exception ex)
{
    _logger.LogError(ex, "Error during login");
    return false;
}
```

**Correção:**
```csharp
// ✅ Tratar cada tipo com a ação adequada
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Network error during login");
    return false;
}
catch (TaskCanceledException)
{
    _logger.LogWarning("Login request timed out");
    return false;
}
catch (JsonException ex)
{
    _logger.LogError(ex, "Invalid response format during login — possible spoofing");
    return false;
}
```

**Esforço:** Médio | **Impacto:** Médio

---

### [SEC-10] Ausência de `CancellationToken` em operações de `AuthService`

**Arquivo:** `CameraApp/Services/AuthService.cs`  
**OWASP:** A05 — Security Misconfiguration

**Problema:** Chamadas HTTP sem `CancellationToken` não podem ser canceladas quando o usuário navega para outra tela ou o app é suspenso, causando leaks de recursos e possível uso de tokens de resposta após o contexto ter mudado.

**Correção:**
```csharp
// ✅ Propagar CancellationToken em todos os métodos HTTP
public async Task<bool> LoginAsync(
    string username, string password, CancellationToken cancellationToken = default)
{
    var response = await _httpClient.PostAsJsonAsync(
        ApiConfig.LoginEndpoint, loginRequest, cancellationToken);
}

public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
{
    var response = await _httpClient.PostAsJsonAsync(
        ApiConfig.RefreshEndpoint, refreshRequest, cancellationToken);
}
```

**Esforço:** Baixo | **Impacto:** Médio

---

## 🔵 Sugestões de Hardening (Baixa Severidade)

---

### [SEC-11] Falta de ofuscação de código para release Android

**Arquivo:** `CameraApp/CameraApp.csproj`

**Problema:** Sem ProGuard/R8, o APK/AAB de produção contém nomes de classes, métodos e constantes legíveis, facilitando engenharia reversa para descobrir endpoints de API e lógica de autenticação.

**Correção:**
```xml
<!-- ✅ CameraApp.csproj -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <AndroidEnableProguard>true</AndroidEnableProguard>
    <AndroidLinkMode>SdkOnly</AndroidLinkMode>
</PropertyGroup>
```

---

### [SEC-12] Ausência de certificate pinning para endpoints críticos

**Arquivo:** `CameraApp/MauiProgram.cs`

**Problema:** Sem pinning, um certificado de CA comprometido ou uma CA maliciosa instalada no dispositivo permite MITM transparente, especialmente relevante para o endpoint de login.

**Correção:**
```csharp
// ✅ MauiProgram.cs — certificate pinning via HttpClientHandler
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        if (errors != SslPolicyErrors.None) return false;
        // Verificar fingerprint SHA-256 do certificado esperado
        const string expectedThumbprint = "AA:BB:CC:DD:..."; // substituir pelo real
        return cert?.GetCertHashString(HashAlgorithmName.SHA256)
                   ?.Equals(expectedThumbprint, StringComparison.OrdinalIgnoreCase) == true;
    }
};
services.AddSingleton(handler);
services.AddHttpClient<IAuthService, AuthService>()
        .ConfigurePrimaryHttpMessageHandler<HttpClientHandler>();
```

> ⚠️ Atenção: certificate pinning requer processo de atualização do pin quando o certificado renovar.

---

### [SEC-13] `HttpClient` sem `Timeout` e sem limite de buffer de resposta

**Arquivo:** `CameraApp/MauiProgram.cs`

**Problema:** Sem timeout, requisições podem travar indefinidamente. Sem `MaxResponseContentBufferSize`, respostas grandes consomem memória livremente.

**Correção:**
```csharp
// ✅ MauiProgram.cs — configurar timeout e limite de buffer
services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(ApiConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.MaxResponseContentBufferSize = 5 * 1024 * 1024; // 5 MB
});
```

---

### [SEC-14] Verificação explícita de `IsSuccessStatusCode` em `BaseService`

**Arquivo:** `CameraApp/Services/BaseService.cs`

**Problema:** Usar apenas `EnsureSuccessStatusCode()` não permite tratamento personalizado por código HTTP (403 vs 404 vs 429 rate-limit), o que pode mascarar respostas de segurança específicas.

**Correção:**
```csharp
// ✅ Verificação explícita com tratamento por status
protected async Task<T?> GetAsync<T>(
    string endpoint, CancellationToken cancellationToken = default)
{
    var response = await _httpClient.GetAsync(endpoint, cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
        _logger.LogWarning("HTTP {StatusCode} from {Endpoint}",
            response.StatusCode, endpoint);
        return default;
    }

    return await response.Content
        .ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
}
```

---

### [SEC-15] iOS — Verificar `NSAllowsArbitraryLoads` no `Info.plist`

**Arquivo:** `CameraApp/Platforms/iOS/Info.plist`

**Problema:** Se `NSAllowsArbitraryLoads = true` estiver no `NSAppTransportSecurity`, o iOS desativa o App Transport Security (ATS), permitindo conexões HTTP abertas e agravando SEC-01 no iOS.

**Verificação e correção:**
```xml
<!-- ✅ Info.plist — deve estar ausente ou configurado assim -->
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <false/>
</dict>
```

---

## ✅ Conformidades Positivas Identificadas

| Aspecto                                                              | Arquivo                                      |
|----------------------------------------------------------------------|----------------------------------------------|
| ✅ Tokens armazenados em `SecureStorage` (não em `Preferences`)      | `Services/AuthService.cs`                    |
| ✅ Refresh token também armazenado em `SecureStorage`                | `Services/AuthService.cs`                    |
| ✅ `CancellationToken` presente em `BaseService`                     | `Services/BaseService.cs`                    |
| ✅ Sem `WebView` com conteúdo remoto não sanitizado                  | —                                            |
| ✅ Sem credenciais hardcoded (usuário/senha) no código-fonte         | —                                            |
| ✅ Sem bypass de validação SSL                                        | —                                            |
| ✅ `IAuthService` como interface — facilita mock seguro nos testes   | `Services/IAuthService.cs`                   |
| ✅ `AuthHttpHandler` centraliza injeção do Bearer token              | `Services/AuthHttpHandler.cs`                |
| ✅ Padrão MVVM separa lógica de negócio das Views                    | Toda a estrutura                             |

---

## Roadmap de Implementação

### Fase 1 — Imediata (antes do próximo deploy)
- [ ] **SEC-01** — Trocar `http://` por `https://` em `ApiConfig.cs`
- [ ] **SEC-02** — Externalizar URL com `#if DEBUG / #else`
- [ ] **SEC-03** — Implementar `SemaphoreSlim` em `AuthHttpHandler`
- [ ] **SEC-04** — Remover `usesCleartextTraffic="true"` e criar `network_security_config.xml`

### Fase 2 — Curto prazo (próximo sprint)
- [ ] **SEC-05** — Validação de entrada no `LoginViewModel`
- [ ] **SEC-06** — `JsonSerializerOptions` com `MaxDepth` nos services
- [ ] **SEC-07** — `allowBackup="false"` no manifesto Android
- [ ] **SEC-08** — Remover logging do corpo de erros de autenticação
- [ ] **SEC-09** — Refinar `catch` para tipos específicos de exceção
- [ ] **SEC-10** — Adicionar `CancellationToken` nos métodos de `AuthService`
- [ ] **SEC-13** — Configurar `Timeout` e `MaxResponseContentBufferSize`
- [ ] **SEC-14** — Usar `IsSuccessStatusCode` explícito em `BaseService`
- [ ] **SEC-15** — Verificar e corrigir `NSAllowsArbitraryLoads` no iOS

### Fase 3 — Médio prazo (hardening)
- [ ] **SEC-11** — Habilitar R8/ProGuard nas builds de release
- [ ] **SEC-12** — Avaliar e implementar certificate pinning

---

*Gerado por auditoria automatizada com GitHub Copilot — especialista-seguranca*
