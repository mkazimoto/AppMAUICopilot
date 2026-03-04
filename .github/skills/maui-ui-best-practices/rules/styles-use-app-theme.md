# Rule: Use Centralized Styles and Themes

## Why

Inline styling leads to duplication and inconsistent UI.

## Guidance

- Define colors, fonts, and styles in `Resources/Styles/Styles.xaml`.
- Use `{StaticResource}` instead of inline values.
- Keep platform-specific styles in `Resources/Styles/Platforms`.

## Example (Bad)

```xaml
<Label Text="Hello" TextColor="Red" FontSize="18" />
```

## Example (Good)

```xaml
<Label Text="Hello" Style="{StaticResource BodyLabel}" />
```
