using System.Globalization;

namespace CameraApp.Converters;

public class IndexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue && intValue > 0)
        {
            return intValue - 1; // Converte ID (1-based) para índice (0-based)
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index && index >= 0)
        {
            return index + 1; // Converte índice (0-based) para ID (1-based)
        }
        return 1;
    }
}