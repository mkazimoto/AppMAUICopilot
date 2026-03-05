---
name: aplicar-layout-ux
description: 'Aplica ou refatora layout e UX em telas .NET MAUI com foco em hierarquia visual, responsividade, acessibilidade, consistencia de estilos e performance. Inclui XAML, ViewModel e code-behind quando necessario. Use quando pedir: melhorar layout, organizar pagina, ajustar UX, refatorar XAML, padronizar UI.'
argument-hint: 'Informe a tela e o objetivo de UX (ex.: "FormEditPage: reduzir rolagem e destacar campos obrigatorios")'
---

# Skill: Aplicar Layout UX (MAUI)

## Objetivo

Padronizar um fluxo repetivel para aplicar melhorias de layout e UX em telas MAUI, reduzindo retrabalho e regressao visual.

## Quando Usar

- Ao criar uma nova pagina XAML com qualidade visual consistente.
- Ao refatorar uma tela existente com problemas de espacamento, hierarquia ou legibilidade.
- Ao adaptar uma tela para diferentes tamanhos de dispositivo.
- Ao revisar acessibilidade e semantica de componentes.
- Ao ajustar comandos, estados visuais e pequenas regras de apresentacao em ViewModel/code-behind para suportar a UX.

## Entradas Esperadas

- Nome da tela ou arquivo alvo (ex.: `Views/FormEditPage.xaml`).
- Objetivo de UX (ex.: destacar CTA principal, reduzir complexidade visual, melhorar escaneabilidade).
- Restricoes (sem alterar fluxo funcional, manter bindings existentes, manter estilo do projeto).
- Escopo de alteracao permitido em C# (ex.: somente propriedades de estado/comando; sem logica de negocio).

## Procedimento

1. Levantar contexto da tela
- Identificar objetivo da pagina, principal acao do usuario e estados da UI (vazio, carregando, erro, sucesso).
- Mapear componentes atuais e bindings antes de editar.

2. Definir hierarquia visual
- Determinar 1 acao principal e acoes secundarias.
- Reorganizar titulos, secoes e agrupamentos para leitura vertical clara.
- Priorizar `VerticalStackLayout` ou `Grid` simples, evitando aninhamento profundo.

3. Estruturar layout responsivo
- Usar `Grid.RowDefinitions` e `Grid.ColumnDefinitions` com `Auto` e `*`.
- Aplicar `OnPlatform`/`OnIdiom` apenas quando houver necessidade real de ajuste.
- Garantir margens e espacamentos consistentes por toda a tela.

4. Padronizar estilo e recursos
- Reutilizar `StaticResource` existentes antes de criar novos.
- Extrair repeticoes para `Style` no nivel apropriado (pagina ou app).
- Evitar hardcode de cores/tamanhos quando existir token de design no projeto.

5. Melhorar UX de formularios e interacao
- Tornar labels e placeholders claros e orientados a tarefa.
- Destacar validacoes e mensagens de erro proximas ao campo.
- Posicionar CTA principal em area previsivel e com contraste adequado.

6. Sincronizar UX com ViewModel/code-behind (quando necessario)
- Ajustar propriedades observaveis para estados visuais (ex.: carregando, vazio, erro, sucesso).
- Garantir comandos com nomes claros e feedback de execucao (desabilitar durante processamento, texto de acao coerente).
- Manter code-behind enxuto, limitado a comportamento de view e sem mover regra de negocio para a view.

7. Aplicar acessibilidade
- Incluir `SemanticProperties` e nomes significativos para leitores de tela.
- Verificar ordem de leitura coerente e textos compreensiveis.
- Preservar contraste suficiente entre texto e fundo.

8. Validar robustez tecnica
- Confirmar que todos os `StaticResource` usados existem.
- Confirmar que bindings permanecem validos apos reorganizacao visual.
- Validar que ajustes de ViewModel/code-behind nao alteraram fluxo funcional esperado.
- Validar build da app para Android (`dotnet build -t:Run -f net$(NetVersion)-android`).

## Decisoes e Ramificacoes

- Se a tela possui muita densidade de informacao:
  - Quebrar em secoes com titulos curtos e espacamento consistente.
- Se ha mais de uma CTA concorrendo por atencao:
  - Definir uma CTA primaria e reduzir destaque das secundarias.
- Se a tela depende de orientacao horizontal:
  - Avaliar `Grid` com colunas adaptativas e fallback vertical.
- Se faltam estilos reutilizaveis:
  - Criar `Style` reutilizavel e remover repeticao inline.
- Se o XAML exige condicoes visuais complexas:
  - Preferir expor estado simples na ViewModel em vez de logica extensa no code-behind.

## Criterios de Conclusao

- Estrutura visual clara, com foco em tarefa principal.
- Layout sem aninhamento desnecessario e com responsividade aceitavel.
- Estilos reaproveitados e recursos centralizados.
- Acessibilidade minima aplicada (`SemanticProperties`, contraste, leitura).
- Estados de UX sincronizados com ViewModel/code-behind sem regressao de comportamento.
- Tela compilando sem erros de recurso, XAML ou binding.

## Resultado Esperado

- XAML mais limpo e previsivel.
- Melhor experiencia de uso sem alterar regras de negocio.
- Base visual consistente com o restante do app.
