using System.Globalization;

namespace CameraApp.Converters;

/// <summary>
/// Converts a string value to a Boolean indicating whether the string is non-empty.
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared singleton instance of <see cref="StringToBoolConverter" />.
    /// </summary>
    public static readonly StringToBoolConverter Instance = new();

    /// <summary>
    /// Converts a string value to a Boolean that indicates whether the string is non-empty.
    /// </summary>
    /// <param name="value">The string value to evaluate.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> is a non-null, non-empty string; otherwise, <see langword="false" />.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    /// <summary>
    /// Converts a Boolean value back to a string. This conversion is not supported.
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