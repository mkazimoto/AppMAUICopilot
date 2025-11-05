# Renovação Automática de Token - Documentação

## Implementação Concluída

### 1. Método EnsureValidTokenAsync no AuthService
- **Localização**: `AuthService.cs`
- **Funcionalidade**: Verifica se o token atual é válido e renova automaticamente se expirado
- **Retorno**: `bool` indicando se o token é válido ou foi renovado com sucesso

```csharp
public async Task<bool> EnsureValidTokenAsync()
{
    // Se não há token, não pode renovar automaticamente
    if (_currentToken == null) return false;

    // Se o token ainda é válido (não expirou), retorna true
    if (!_currentToken.IsExpired) return true;

    // Token expirado, tenta renovar automaticamente
    var newToken = await RefreshTokenAsync(_currentToken.RefreshToken);
    if (newToken != null)
    {
        return true; // Renovação bem-sucedida
    }
    else
    {
        await LogoutAsync(); // Força logout se renovação falhou
        return false;
    }
}
```

### 2. Integração com FormService
- **EnsureAuthenticatedAsync**: Atualizado para usar `EnsureValidTokenAsync()`
- **Verificação automática**: Antes de cada chamada à API, verifica e renova o token se necessário
- **Tratamento de erro**: Lança `UnauthorizedAccessException` se o token não puder ser renovado

### 3. Tratamento de Respostas 401
- **HandleErrorResponseAsync**: Detecta respostas 401 (Unauthorized)
- **Renovação automática**: Tenta renovar o token quando recebe 401
- **Mensagens específicas**: Informa ao usuário sobre renovação ou necessidade de novo login

### 4. AuthHttpHandler (Preparado para uso futuro)
- **Interceptação de requisições**: Garante token válido antes de enviar
- **Retry automático**: Em caso de 401, tenta renovar e reenviar a requisição
- **Thread-safe**: Usa flag `_isRefreshing` para evitar múltiplas renovações simultâneas

### 5. Fluxo de Renovação Automática

#### Cenário 1: Token Válido
1. User faz ação (ex: carregar formulários)
2. `EnsureValidTokenAsync()` verifica o token
3. Token ainda válido → Continua com a requisição

#### Cenário 2: Token Expirado (Renovação Bem-sucedida)
1. User faz ação
2. `EnsureValidTokenAsync()` detecta token expirado
3. Chama `RefreshTokenAsync()` automaticamente
4. Token renovado e salvo no SecureStorage
5. Continua com a requisição usando novo token

#### Cenário 3: Token Expirado (Renovação Falhada)
1. User faz ação
2. `EnsureValidTokenAsync()` detecta token expirado
3. Tenta renovar mas refresh_token também expirou
4. Chama `LogoutAsync()` automaticamente
5. Dispara evento `AuthenticationChanged(false)`
6. App pode redirecionar para tela de login

#### Cenário 4: Resposta 401 da API
1. Requisição enviada com token aparentemente válido
2. API retorna 401 (token foi invalidado no servidor)
3. `HandleErrorResponseAsync()` detecta 401
4. Tenta renovar automaticamente
5. Se renovado, informa "tente novamente"
6. Se falhou, informa "faça login novamente"

### 6. Configurações de Segurança
- **SecureStorage**: Tokens salvos de forma segura
- **Logout automático**: Em caso de falha na renovação
- **Eventos**: `AuthenticationChanged` notifica mudanças de estado
- **Logs**: Registra tentativas de renovação para debug

### 7. Benefícios para o Usuário
- **Transparente**: Usuário não percebe renovações automáticas
- **Sem interrupções**: Não precisa fazer login novamente durante uso normal
- **Seguro**: Token sempre atualizado, logout automático se necessário
- **Feedback claro**: Mensagens específicas quando login é necessário

### 8. Pontos de Ativação
A renovação automática é ativada em:
- Carregamento de formulários
- Filtros e pesquisas
- Operações CRUD (Create, Update, Delete)
- Qualquer chamada de API através do FormService

### 9. Monitoramento
Para monitorar a renovação automática:
- Verificar logs do AuthService
- Observar eventos `AuthenticationChanged`
- Monitorar mudanças no SecureStorage

### 10. Próximas Melhorias
- Renovação proativa (antes da expiração)
- Cache de múltiplas tentativas
- Métricas de renovação
- Interface para controle manual