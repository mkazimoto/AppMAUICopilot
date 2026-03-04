# Rule: Prefer Reusable Controls

## Why

Reusable components reduce duplication and improve maintainability.

## Guidance

- Place reusable UI in `/Views/Controls`.
- Use `ContentView` for composite controls.
- Avoid repeating the same XAML across pages.

## Example

Create `PrimaryButton.xaml` instead of repeating styled `<Button>` everywhere.
