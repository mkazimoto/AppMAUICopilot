using System.Globalization;

namespace CameraApp.Converters;

/// <summary>
/// Converts between a one-based identifier and a zero-based list index.
/// </summary>
public class IndexConverter : IValueConverter
{
    /// <summary>
    /// Converts a one-based integer identifier to a zero-based list index.
    /// </summary>
    /// <param name="value">The one-based integer ID to convert.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns>The zero-based index corresponding to <paramref name="value" />, or <c>0</c> if the value is not a positive integer.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue && intValue > 0)
        {
            return intValue - 1; // Converte ID (1-based) para índice (0-based)
        }
        return 0;
    }

    /// <summary>
    /// Converts a zero-based list index back to a one-based identifier.
    /// </summary>
    /// <param name="value">The zero-based index to convert.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns>The one-based identifier corresponding to <paramref name="value" />, or <c>1</c> if the value is not a non-negative integer.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index && index >= 0)
        {
            return index + 1; // Converte índice (0-based) para ID (1-based)
        }
        return 1;
    }
}