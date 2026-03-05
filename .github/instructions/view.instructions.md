---
description: "Use ao criar ou refatorar Views .NET MAUI (.xaml e .cs). Diretrizes preferenciais para composicao da View, configuracao de BindingContext, limites do code-behind, gatilhos de navegacao, acessibilidade e verificacoes de validacao de UI."
applyTo: "**/Views/*.{xaml,cs}"
---

# Diretrizes de Views MAUI

Ao gerar ou sugerir codigo em `Views`, use estas convencoes como guideline preferencial do projeto:

## 1. Papel das Views
- Mantenha as Views focadas em apresentacao e ligacao de interacoes do usuario.
- Coloque regras de negocio, acesso a dados e logica de validacao em ViewModels e Services.
- Defina a estrutura da pagina em XAML. Use code-behind apenas para integracao de ciclo de vida da plataforma/view que nao possa ser expressa com bindings.
- Aplique as regras abaixo como convencoes preferenciais do projeto; se um cenario exigir excecao, mantenha esse desvio explicito e minimo.

## 2. BindingContext e compiled bindings
- Defina `x:DataType` na pagina raiz para habilitar compiled bindings e ganhar validacao em tempo de compilacao.
- Nao instancie ViewModels diretamente nas Views; resolva ViewModels via DI e configure o `BindingContext` em um unico ponto.
- Prefira bindings e comandos em vez de manipulacao direta de controles no code-behind.
- Use `x:Name` apenas quando um elemento realmente precisar ser acessado no code-behind.

## 3. Navegacao
- Use rotas do `Shell` para navegacao.
- Sempre que possivel, dispare navegacao por comandos no ViewModel em vez de no code-behind da View.

## 4. Estilos e reutilizacao visual
- Reutilize regras visuais com estilos e dicionarios de recursos em vez de repetir propriedades inline.
- Use `Border` no lugar de `Frame` para conteineres com borda.
- Centralize definicoes de tema em `Resources/Styles.xaml`.

## 5. Acessibilidade
- Inclua metadados de acessibilidade (`SemanticProperties.Description`, `SemanticProperties.Hint` e hierarquia significativa de titulos) para elementos interativos e informativos principais.
- Garanta que todo controle interativo tenha uma estrategia de seletor estavel (`AutomationId` ou descricao semantica) para suportar automacao de testes de UI.

## 6. Layout e performance
- Mantenha a profundidade do layout baixa.
- Prefira `VerticalStackLayout` e `HorizontalStackLayout` em vez de arvores de `Grid` profundamente aninhadas quando entregarem comportamento equivalente.
- Use `CollectionView` em vez de `ListView` para listas dinamicas.

## 7. Validacao de UI e automacao de testes
- Valide estados visuais que impactam usabilidade (`IsEnabled`, estados de carregamento, estados vazios e estados de erro) com bindings deterministicos.
- Mantenha textos exibidos ao usuario em resources sempre que possivel para tornar localizacao e verificacoes de regressao de UI mais confiaveis.

## 8. Documentacao e estilo C#
- Documente membros publicos nao triviais com comentarios XML (`///`).
- Prefira `file-scoped namespace` quando consistente com o arquivo.
- Use nomes claros: PascalCase para tipos/membros publicos e `_camelCase` para campos privados.
