using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

[TestClass]
public class StringToBoolConverterTests
{
    private StringToBoolConverter _converter = null!;

    [TestInitialize]
    public void Setup()
    {
        _converter = new StringToBoolConverter();
    }

    #region Instance Tests

    [TestMethod]
    public void Instance_IsNotNull()
    {
        // Assert
        Assert.IsNotNull(StringToBoolConverter.Instance);
    }

    [TestMethod]
    public void Instance_IsSingleton()
    {
        // Arrange & Act
        var instance1 = StringToBoolConverter.Instance;
        var instance2 = StringToBoolConverter.Instance;

        // Assert
        Assert.AreSame(instance1, instance2);
    }

    #endregion

    #region Convert Tests - String Values

    [TestMethod]
    public void Convert_WhenValueIsNonEmptyString_ReturnsTrue()
    {
        // Arrange
        string value = "Hello World";

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsEmptyString_ReturnsFalse()
    {
        // Arrange
        string value = "";

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsWhitespaceString_ReturnsTrue()
    {
        // Arrange
        string value = "   ";

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsSingleCharacter_ReturnsTrue()
    {
        // Arrange
        string value = "A";

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsLongString_ReturnsTrue()
    {
        // Arrange
        string value = new string('A', 1000);

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    #endregion

    #region Convert Tests - Null Values

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

    #endregion

    #region Convert Tests - Non-String Values

    [TestMethod]
    public void Convert_WhenValueIsInteger_ReturnsTrue()
    {
        // Arrange
        int value = 42;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsZero_ReturnsTrue()
    {
        // Arrange
        int value = 0;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsDouble_ReturnsTrue()
    {
        // Arrange
        double value = 3.14;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsBoolean_ReturnsTrue()
    {
        // Arrange
        bool value = true;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsFalseBoolean_ReturnsTrue()
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
    public void Convert_WhenValueIsDateTime_ReturnsTrue()
    {
        // Arrange
        DateTime value = DateTime.Now;

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsGuid_ReturnsTrue()
    {
        // Arrange
        Guid value = Guid.NewGuid();

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsCustomObject_ReturnsTrue()
    {
        // Arrange
        var value = new { Name = "Test", Age = 30 };

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsObjectWithEmptyToString_ReturnsFalse()
    {
        // Arrange
        var value = new ObjectWithEmptyToString();

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsObjectWithNullToString_ReturnsFalse()
    {
        // Arrange
        var value = new ObjectWithNullToString();

        // Act
        var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(false, result);
    }

    #endregion

    #region Convert Tests - Culture and Parameter

    [TestMethod]
    public void Convert_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        string value = "Test";
        var culture = new CultureInfo("pt-BR");

        // Act
        var result = _converter.Convert(value, typeof(bool), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Convert_WithParameter_IgnoresParameter()
    {
        // Arrange
        string value = "Test";
        object parameter = "some parameter";

        // Act
        var result = _converter.Convert(value, typeof(bool), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<bool>(result);
        Assert.AreEqual(true, result);
    }

    #endregion

    #region ConvertBack Tests

    [TestMethod]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        bool value = true;

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertBack_WithNullValue_ThrowsNotImplementedException()
    {
        // Arrange
        object? value = null;

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture));
    }

    #endregion

    #region Helper Classes

    private class ObjectWithEmptyToString
    {
        public override string ToString() => string.Empty;
    }

    private class ObjectWithNullToString
    {
        public override string? ToString() => null;
    }

    #endregion
}
