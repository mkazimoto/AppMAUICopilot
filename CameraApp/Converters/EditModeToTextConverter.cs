using System.Globalization;

namespace CameraApp.Converters;

/// <summary>
/// Converts a Boolean edit-mode flag to a localized action button label.
/// </summary>
public class EditModeToTextConverter : IValueConverter
{
    /// <summary>
    /// Converts a Boolean edit-mode value to the corresponding button text.
    /// </summary>
    /// <param name="value">The Boolean value indicating whether edit mode is active.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns><c>"Atualizar Formulário"</c> when <paramref name="value" /> is <see langword="true" />; <c>"Criar Formulário"</c> when <see langword="false" />; <c>"Salvar"</c> if the value is not a Boolean.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEditMode)
        {
            return isEditMode ? "Atualizar Formulário" : "Criar Formulário";
        }
        return "Salvar";
    }

    /// <summary>
    /// Converts a button label back to a Boolean edit-mode value. This conversion is not supported.
    /// </summary>
    /// <param name="value">The value to convert back (unused).</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <exception cref="NotImplementedException">Always thrown; back-conversion is not supported.</exception>
    /// <returns>This method never returns normally.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}