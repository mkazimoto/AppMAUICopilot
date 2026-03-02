using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

public class InvertedBoolConverterTests
{
    private readonly InvertedBoolConverter _converter = new();

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Convert_Bool_ReturnsInverted(bool input, bool expected)
    {
        var result = _converter.Convert(input, typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_NonBool_ReturnsFalse()
    {
        var result = _converter.Convert("not a bool", typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_Null_ReturnsFalse()
    {
        var result = _converter.Convert(null, typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(false, result);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ConvertBack_Bool_ReturnsInverted(bool input, bool expected)
    {
        var result = _converter.ConvertBack(input, typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }
}
