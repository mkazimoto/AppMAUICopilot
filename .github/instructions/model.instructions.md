---
description: "Use ao criar, refatorar ou revisar classes em Models no MAUI. Cobre contratos de dados, serializacao JSON, nulabilidade, validacao leve e consistencia de estado."
name: "Diretrizes de Models MAUI"
applyTo: "**/Models/*.cs"
---

# Instrucoes para Models (MAUI)

Ao gerar ou sugerir codigo em `Models`, use estas convencoes como guideline preferencial do projeto:

## 1. Papel dos Models
- Trate models como contratos de dados (API/estado), evitando dependencia de UI, navegacao, DI ou servicos.
- Nao adicione chamadas HTTP, acesso a banco, logging ou regras de fluxo de tela em `Models`.
- Mantenha regras de negocio e logica derivada fora do model, centralizando em `Services`, `ViewModels` ou utilitarios dedicados.

## 2. Serializacao e compatibilidade de API
- Use `System.Text.Json` e `[JsonPropertyName("...")]` quando o nome da propriedade C# diferir do payload.
- Evite hardcode de nomes de campos em strings espalhadas; prefira atributos de serializacao no proprio modelo.
- Ao evoluir contratos, preserve compatibilidade retroativa quando houver consumidores legados.

## 3. Nulabilidade e valores padrao
- Explicite nulabilidade (`string?`, `DateTime?`, etc.) para campos opcionais do payload.
- Inicialize colecoes com `new()` e strings obrigatorias com `string.Empty` para evitar `NullReferenceException`.
- Para datas/horarios de API, prefira consistencia em UTC e documente quando um campo for horario local.

## 4. Heranca e tipos base
- Entidades de dominio compartilhadas devem herdar de `BaseEntity` quando fizer sentido no contrato da API.
- Nao force heranca em DTOs que nao possuem identidade/auditoria comum.
- Use tipos especificos para requests/responses quando melhorarem clareza (ex.: `LoginRequest`, `FormResponse`).

## 5. Estrutura do model
- Mantenha models estritamente como estrutura de dados (propriedades e, quando necessario, atributos de serializacao).
- Evite adicionar metodos em models, inclusive helpers (`Clone`, `Reset`, builders de query e similares).
- Se houver necessidade de transformacao, formatacao ou regra derivada, mova a implementacao para `Service`, `ViewModel` ou utilitario dedicado.

## 6. Estilo e documentacao C#
- Documente tipos e membros publicos nao triviais com comentarios XML (`///`).
- Prefira `file-scoped namespace` quando consistente com o arquivo.
- Use PascalCase para tipos/propriedades/metodos e `_camelCase` para campos privados.
- Evite comentarios redundantes; mantenha o codigo autoexplicativo com nomes claros.
