using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

public class StringToBoolConverterTests
{
    private readonly StringToBoolConverter _converter = new();

    [Theory]
    [InlineData("hello", true)]
    [InlineData("a", true)]
    [InlineData(" ", true)]
    public void Convert_NonEmptyString_ReturnsTrue(string input, bool expected)
    {
        var result = _converter.Convert(input, typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Convert_NullOrEmptyString_ReturnsFalse(string? input, bool expected)
    {
        var result = _converter.Convert(input, typeof(bool), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(true, typeof(string), null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(StringToBoolConverter.Instance, StringToBoolConverter.Instance);
    }
}
