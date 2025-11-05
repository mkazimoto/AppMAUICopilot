# Sistema de Autenticação TOTVS RM

Este projeto implementa um sistema completo de autenticação Bearer para APIs do TOTVS RM seguindo as práticas recomendadas do .NET MAUI.

## 🔒 Funcionalidades Implementadas

- ✅ **Autenticação Bearer**: Login com token JWT
- ✅ **Logout Seguro**: Limpeza completa de credenciais
- ✅ **Armazenamento Seguro**: Tokens salvos com SecureStorage
- ✅ **Renovação Automática**: Refresh token implementado
- ✅ **Permissões HTTP**: Configurado para aceitar HTTP no Android
- ✅ **Interface Responsiva**: UI moderna com Border (NET 9)
- ✅ **Validação de Formulário**: Campos obrigatórios e estados de loading

## 📱 Como Usar

### 1. Página de Login
Acesse a aba **"Login"** no aplicativo para:
- Inserir credenciais (usuário e senha)
- Configurar Service Alias (opcional)
- Fazer login e receber token Bearer
- Fazer logout e limpar credenciais

### 2. Configuração do Servidor
No `AuthService.cs`, ajuste a URL base:
```csharp
private readonly string _baseUrl = "http://localhost:8051"; // Seu servidor RM
```

### 3. Credenciais Padrão TOTVS
- **Usuário**: `mestre`
- **Senha**: `totvs`
- **Service Alias**: `CorporeRM` (opcional)

## 🔧 Configurações Técnicas

### Android - Permissões HTTP
O arquivo `AndroidManifest.xml` está configurado com:
```xml
<application android:usesCleartextTraffic="true">
```

### Estrutura do Token
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "expires_in": 300,
  "refresh_token": "abc123..."
}
```

### Endpoints da API
- **Login**: `POST /api/connect/token`
- **Refresh**: `POST /api/connect/token` (com refresh_token)

## 📚 Arquitetura

### Serviços
- **`IAuthService`**: Interface para autenticação
- **`AuthService`**: Implementação com HttpClient
- **`AuthHttpHandler`**: Interceptor para adicionar token automaticamente

### ViewModels
- **`LoginViewModel`**: Lógica de login/logout com Commands

### Views
- **`LoginPage`**: Interface de usuário responsiva

### Models
- **`AuthToken`**: Representação do token JWT
- **`LoginRequest`**: Dados para requisição de login
- **`RefreshTokenRequest`**: Dados para renovação

## 🚀 Exemplo de Uso em Código

### Fazer Login
```csharp
var authService = serviceProvider.GetService<IAuthService>();
var token = await authService.LoginAsync("mestre", "totvs", "CorporeRM");
```

### Verificar Autenticação
```csharp
if (authService.IsAuthenticated)
{
    var currentToken = authService.CurrentToken;
    // Usar token nas requisições
}
```

### Fazer Logout
```csharp
await authService.LogoutAsync();
```

## 🔒 Segurança

- Tokens armazenados com `SecureStorage`
- Renovação automática antes da expiração
- Limpeza completa no logout
- Validação de expiração de token
- Interceptor HTTP para requisições automáticas

## 📋 Estados da Interface

- **Não Autenticado**: Mostra formulário de login
- **Carregando**: Indicador de atividade durante autenticação
- **Autenticado**: Mostra informações de sucesso e botão de logout
- **Erro**: Exibe mensagens de erro em vermelho

## 🎨 Design System

- Usa Border em vez de Frame (NET 9)
- Cores dinâmicas do tema
- Ícones e emojis para UX
- Layout responsivo com Grid
- Converters para binding avançado

## 🔄 Fluxo de Autenticação

1. Usuário insere credenciais
2. App faz POST para `/api/connect/token`
3. Recebe access_token e refresh_token
4. Salva tokens no SecureStorage
5. Adiciona Bearer token em todas as requisições
6. Renova token automaticamente quando necessário
7. Logout limpa todos os dados

Este sistema está pronto para produção e segue as melhores práticas de segurança para aplicações móveis.