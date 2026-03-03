---
description: "Instruções para Views em XAML no projeto MAUI, seguindo as melhores práticas de organização, nomenclatura e estilo."
applyTo: "**/Views/*.{cs,xaml}"
---

# Instruções para código XAML (MAUI)

Ao gerar ou sugerir código XAML, siga estas convenções:

## 1. Nomenclatura
- Use `x:Name` para identificar elementos que serão referenciados no code-behind.
- Use `x:Key` apenas para recursos (dentro de `<ResourceDictionary>`).
- Prefira nomes em **PascalCase** para elementos e **camelCase** para nomes de binding.

## 2. Organização de propriedades
- Propriedades essenciais (como `Text`, `Command`) primeiro.
- Propriedades de layout (`Grid.Row`, `Margin`, `HorizontalOptions`) depois.
- Eventos por último.

## 3. Recursos e Estilos
- Sempre que uma propriedade se repetir, crie um `<Style>` implícito ou explícito.
- Coloque estilos em dicionários de recursos no nível adequado (App.xaml, página, ou arquivo separado).

## 4. Espaçamento e indentação
- Use 4 espaços por nível de indentação.
- Mantenha atributos em linhas separadas se a linha exceder 120 caracteres.
- Feche tags auto-contidas com `/>` e tags com conteúdo com `</Elemento>`.

## 5. Boas práticas MAUI
- Prefira `ColumnDefinitions` e `RowDefinitions` com `Auto`, `*` e números absolutos.
- Use `OnPlatform` para ajustar valores por plataforma quando necessário.
- Aproveite os layouts específicos (VerticalStackLayout, HorizontalStackLayout) em vez de Grids complexos.

## 6. Comentários
- Comente apenas quando a lógica não for óbvia (ex: "Esta linha força o recálculo do layout").
- Use comentários em português, mas mantenha consistência no projeto.