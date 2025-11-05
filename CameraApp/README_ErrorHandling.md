# Tratamento de Erros da API - Documentação

## Implementação Concluída

### 1. Modelo de Erro da API
- **ApiError.cs**: Representa o formato JSON de erro retornado pela API TOTVS
- **ApiException.cs**: Exceção customizada para erros específicos da API

### 2. Formato do Erro FE018
```json
{
  "code": "FE018",
  "message": "An expression of non-boolean type specified in a context where a condition is expected, near ')'.",
  "detailedMessage": "",
  "helpUrl": "",
  "details": null
}
```

### 3. Tratamento nos Serviços
- **FormService.cs**: Todos os métodos agora verificam respostas de erro e lançam `ApiException`
- **HandleErrorResponseAsync()**: Método que deserializa erros da API e cria exceções apropriadas

### 4. Tratamento nos ViewModels
- **FormListViewModel.cs**: Métodos atualizados para capturar `ApiException` e exibir mensagens específicas
- **ShowErrorAsync()**: Método auxiliar que exibe mensagens de erro formatadas

### 5. Experiência do Usuário
Quando o erro FE018 ocorrer, o usuário verá:

**Título**: "Erro da API (FE018)"
**Mensagem**: "An expression of non-boolean type specified in a context where a condition is expected, near ')'."
**Dica adicional**: "Verifique se os filtros estão corretos ou tente limpar os filtros."

### 6. Pontos de Captura
- Carregamento de formulários
- Filtros por categoria
- Filtros por status  
- Filtros por período de data
- Paginação (carregar mais)

### 7. Próximos Passos
Para casos específicos do erro FE018:
1. Validar filtros ODATA antes de enviar
2. Implementar sanitização de parâmetros
3. Adicionar logs detalhados para debug
4. Criar fallbacks para filtros inválidos

### 8. Como Testar
1. Execute filtros com valores inválidos
2. Use categorias ou status inexistentes
3. Teste filtros de data malformados
4. Verifique se as mensagens de erro aparecem corretamente