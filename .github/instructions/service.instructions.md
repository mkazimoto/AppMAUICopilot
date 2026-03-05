---
description: "Instrucoes para Services em C# no projeto MAUI, com foco em DI, chamadas HTTP, tratamento de erros e padrao de contratos."
applyTo: "**/Services/*.cs"
---

# Instrucoes para Services (MAUI)

Ao gerar ou sugerir codigo em `Services`, siga estas convencoes:

## 1. Contratos e injecao de dependencia
- Sempre exponha servicos por interface (ex.: `IFormService`, `IAuthService`).
- Injete dependencias via construtor, nunca instancie `HttpClient` diretamente dentro de metodos.
- Mantenha o registro no `MauiProgram.cs` alinhado com as interfaces implementadas.

## 2. Assinaturas de metodos
- Use sufixo `Async` em metodos assincronos.
- Prefira `Task<T>` e `Task` para operacoes I/O.
- Em contratos publicos, explicite nulabilidade no retorno (`T?`) quando aplicavel.

## 3. Chamadas de API e configuracao
- Construa URLs usando `ApiConfig.BaseUrl` e `ApiConfig.Endpoints` (ou `EndpointPath` em servicos base).
- Nao hardcode URL, timeout, headers ou nomes de endpoints fora de `Config/ApiConfig.cs`.
- Utilize `ApiConfig.GetJsonOptions()` para serializacao/deserializacao quando disponivel.

## 4. Tratamento de erros
- Em respostas HTTP nao exitosas, delegue para um metodo de tratamento (ex.: `HandleErrorResponseAsync`).
- Lance `ApiException` com contexto suficiente (status code, payload e operacao quando possivel).
- Nao silencie excecoes; preserve stack trace com `throw;` ao repropagar `ApiException`.

## 5. Logging e diagnostico
- Prefira `ILogger<T>` para logs de informacao, aviso e erro.
- Evite `Console.WriteLine` em services de producao; use logging estruturado.
- Use mensagens de log objetivas com placeholders nomeados.

## 6. Organizacao e responsabilidade
- Mantenha servicos focados em regras de acesso a dados/API e orquestracao tecnica.
- Nao mover logica de UI para services.
- Para CRUD padrao, prefira herdar de `BaseService<T>` em vez de duplicar implementacao.

## 7. Documentacao e estilo C#
- Documente interfaces e membros publicos com comentarios XML (`///`).
- Use `file-scoped namespace` quando consistente com o arquivo.
- Mantenha nomes em PascalCase para tipos/metodos/propriedades e `_camelCase` para campos privados.
