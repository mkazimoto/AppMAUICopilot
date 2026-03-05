---
description: "Use ao criar ou refatorar Views .NET MAUI (.xaml e .cs). Diretrizes preferenciais para composicao da View, configuracao de BindingContext, limites do code-behind, gatilhos de navegacao, acessibilidade e verificacoes de validacao de UI."
name: "Diretrizes de Views MAUI"
applyTo: "**/Views/*.{xaml,cs}"
---

# Diretrizes de Views MAUI

- Aplique as regras abaixo como convencoes preferenciais do projeto. Se um cenario exigir excecao, mantenha esse desvio explicito e minimo.
- Mantenha as Views focadas em apresentacao e ligacao de interacoes. Coloque regras de negocio, acesso a dados e logica de validacao em ViewModels e Services.
- Defina a estrutura da pagina primeiro em XAML. Use code-behind apenas para integracao de ciclo de vida da plataforma/view que nao possa ser expressa com bindings.
- Defina `x:DataType` na pagina raiz para habilitar compiled bindings.
- Prefira bindings e comandos em vez de manipulacao direta de controles no code-behind.
- Use `x:Name` apenas quando um elemento realmente precisar ser acessado no code-behind.
- Nao instancie ViewModels diretamente nas Views. Resolva ViewModels via DI e configure o `BindingContext` em um unico ponto.
- Use rotas do `Shell` para navegacao. Sempre que possivel, dispare navegacao por comandos no ViewModel.
- Reutilize regras visuais com estilos e dicionarios de recursos em vez de repetir propriedades inline.
- Use `Border` no lugar de `Frame` para conteineres com borda.
- Inclua metadados de acessibilidade (`SemanticProperties.Description`, `SemanticProperties.Hint` e hierarquia significativa de titulos) para elementos interativos e informativos principais.
- Mantenha a profundidade do layout baixa. Prefira `VerticalStackLayout` e `HorizontalStackLayout` em vez de arvores de `Grid` profundamente aninhadas quando entregarem comportamento equivalente.

## Verificacoes de Validacao de UI

- Garanta que todo controle interativo tenha uma estrategia de seletor estavel (`x:Name`, `AutomationId` ou descricao semantica) para suportar automacao de testes de UI.
- Valide estados visuais que impactam usabilidade (`IsEnabled`, estados de carregamento, estados vazios e estados de erro) com bindings deterministicos.
- Mantenha textos exibidos ao usuario em resources sempre que possivel para tornar localizacao e verificacoes de regressao de UI mais confiaveis.
