using System.Globalization;

namespace CameraApp.Converters;

/// <summary>
/// Converts a Boolean value to its logical inverse.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts a Boolean value to its logical inverse.
    /// </summary>
    /// <param name="value">The Boolean value to invert.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> is <see langword="false" />; <see langword="false" /> if <paramref name="value" /> is <see langword="true" />; <see langword="false" /> if the value is not a Boolean.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return false;
    }

    /// <summary>
    /// Converts an inverted Boolean value back to its original value.
    /// </summary>
    /// <param name="value">The Boolean value to invert.</param>
    /// <param name="targetType">The target binding type (unused).</param>
    /// <param name="parameter">An optional converter parameter (unused).</param>
    /// <param name="culture">The culture to use in the converter (unused).</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> is <see langword="false" />; <see langword="false" /> if <paramref name="value" /> is <see langword="true" />; <see langword="false" /> if the value is not a Boolean.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;

        return false;
    }
}