# Rule: Use Compiled Bindings

## Why

Compiled bindings reduce runtime overhead and improve performance.

## Guidance

- Use `x:DataType` on root layout.
- Avoid `{Binding}` without a data type.

## Example (Good)

```xaml
<ContentPage xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" ...
             x:DataType="vm:MainViewModel">
  <Label Text="{Binding Title}" />
</ContentPage>
```
