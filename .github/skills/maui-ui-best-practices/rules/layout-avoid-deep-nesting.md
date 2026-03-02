# Rule: Avoid Deep Layout Nesting

## Why

Deeply nested layouts cause performance issues and make UI harder to maintain.

## Guidance

- Prefer `Grid` or `FlexLayout` over multiple nested `StackLayout`s.
- Limit nesting to 3 levels whenever possible.
- Use `Grid` for structured layouts instead of stacking multiple containers.

## Example (Bad)

```xaml
<StackLayout>
  <StackLayout>
    <StackLayout>
      <Label Text="Hello" />
    </StackLayout>
  </StackLayout>
</StackLayout>
```

## Example (Good)

```xaml
<Grid>
  <Label Text="Hello" />
</Grid>
```
