using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

[TestClass]
public class InvertedBoolConverterTests
{
    private InvertedBoolConverter _converter = null!;

    [TestInitialize]
    public void Setup()
    {
        _converter = new InvertedBoolConverter();
    }

    #region Convert Tests

    [TestMethod]
    public void Convert_WhenValueIsTrue_ReturnsFalse()
    {
        // Arrange
        bool value = true;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsFalse_ReturnsTrue()
    {
        // Arrange
        bool value = false;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsNull_ReturnsFalse()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsNotBoolean_ReturnsFalse()
    {
        // Arrange
        object value = "not a boolean";

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsInteger_ReturnsFalse()
    {
        // Arrange
        object value = 42;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        bool value = true;
        var culture = new CultureInfo("pt-BR");

        // Act
        var result = _converter.Convert(value, typeof(bool), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WithParameter_IgnoresParameter()
    {
        // Arrange
        bool value = true;
        object parameter = "some parameter";

        // Act
        var result = _converter.Convert(value, typeof(bool), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    #endregion

    #region ConvertBack Tests

    [TestMethod]
    public void ConvertBack_WhenValueIsTrue_ReturnsFalse()
    {
        // Arrange
        bool value = true;

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsFalse_ReturnsTrue()
    {
        // Arrange
        bool value = false;

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsNull_ReturnsFalse()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsNotBoolean_ReturnsFalse()
    {
        // Arrange
        object value = "not a boolean";

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void ConvertBack_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        bool value = true;
        var culture = new CultureInfo("fr-FR");

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void ConvertBack_WithParameter_IgnoresParameter()
    {
        // Arrange
        bool value = false;
        object parameter = "some parameter";

        // Act
        var result = _converter.ConvertBack(value, typeof(bool), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    #endregion

    #region Symmetry Tests

    [TestMethod]
    public void Convert_AndConvertBack_AreSymmetric_ForTrueValue()
    {
        // Arrange
        bool originalValue = true;

        // Act
        var converted = _converter.Convert(originalValue, typeof(bool), null, CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalValue, convertedBack);
    }

    [TestMethod]
    public void Convert_AndConvertBack_AreSymmetric_ForFalseValue()
    {
        // Arrange
        bool originalValue = false;

        // Act
        var converted = _converter.Convert(originalValue, typeof(bool), null, CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalValue, convertedBack);
    }

    #endregion
}
