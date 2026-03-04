# Rule: Always Provide Semantic Properties

## Why

Accessibility is essential for screen readers and platform assistive technologies.

## Guidance

- Use `SemanticProperties.Description` for interactive elements.
- Use `AutomationId` for testing and accessibility tools.
- Ensure color contrast meets WCAG AA.

## Example

```xaml
<Button
  Text="Submit"
  SemanticProperties.Description="Submit form"
  AutomationId="SubmitButton" />
```
