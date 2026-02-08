using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

[TestClass]
public class IndexConverterTests
{
    private IndexConverter _converter = null!;

    [TestInitialize]
    public void Setup()
    {
        _converter = new IndexConverter();
    }

    #region Convert Tests (ID to Index: 1-based to 0-based)

    [TestMethod]
    public void Convert_WhenValueIsOne_ReturnsZero()
    {
        // Arrange
        int value = 1;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsTwo_ReturnsOne()
    {
        // Arrange
        int value = 2;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsTen_ReturnsNine()
    {
        // Arrange
        int value = 10;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(9, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsLargeNumber_ReturnsCorrectIndex()
    {
        // Arrange
        int value = 1000;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(999, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsZero_ReturnsZero()
    {
        // Arrange
        int value = 0;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsNegative_ReturnsZero()
    {
        // Arrange
        int value = -5;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsNull_ReturnsZero()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsString_ReturnsZero()
    {
        // Arrange
        object value = "not an integer";

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsDouble_ReturnsZero()
    {
        // Arrange
        object value = 5.5;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WhenValueIsBoolean_ReturnsZero()
    {
        // Arrange
        object value = true;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Convert_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        int value = 5;
        var culture = new CultureInfo("pt-BR");

        // Act
        var result = _converter.Convert(value, typeof(int), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void Convert_WithParameter_IgnoresParameter()
    {
        // Arrange
        int value = 3;
        object parameter = "some parameter";

        // Act
        var result = _converter.Convert(value, typeof(int), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(2, result);
    }

    #endregion

    #region ConvertBack Tests (Index to ID: 0-based to 1-based)

    [TestMethod]
    public void ConvertBack_WhenValueIsZero_ReturnsOne()
    {
        // Arrange
        int value = 0;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsOne_ReturnsTwo()
    {
        // Arrange
        int value = 1;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsNine_ReturnsTen()
    {
        // Arrange
        int value = 9;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(10, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsLargeNumber_ReturnsCorrectID()
    {
        // Arrange
        int value = 999;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1000, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsNegative_ReturnsOne()
    {
        // Arrange
        int value = -5;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsNull_ReturnsOne()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsString_ReturnsOne()
    {
        // Arrange
        object value = "not an integer";

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsDouble_ReturnsOne()
    {
        // Arrange
        object value = 5.5;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WhenValueIsBoolean_ReturnsOne()
    {
        // Arrange
        object value = true;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ConvertBack_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        int value = 4;
        var culture = new CultureInfo("fr-FR");

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void ConvertBack_WithParameter_IgnoresParameter()
    {
        // Arrange
        int value = 2;
        object parameter = "some parameter";

        // Act
        var result = _converter.ConvertBack(value, typeof(int), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(3, result);
    }

    #endregion

    #region Symmetry Tests

    [TestMethod]
    public void Convert_AndConvertBack_AreSymmetric_ForValueOne()
    {
        // Arrange
        int originalValue = 1;

        // Act
        var converted = _converter.Convert(originalValue, typeof(int), null, CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalValue, convertedBack);
    }

    [TestMethod]
    public void Convert_AndConvertBack_AreSymmetric_ForValueFive()
    {
        // Arrange
        int originalValue = 5;

        // Act
        var converted = _converter.Convert(originalValue, typeof(int), null, CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalValue, convertedBack);
    }

    [TestMethod]
    public void Convert_AndConvertBack_AreSymmetric_ForLargeValue()
    {
        // Arrange
        int originalValue = 100;

        // Act
        var converted = _converter.Convert(originalValue, typeof(int), null, CultureInfo.InvariantCulture);
        var convertedBack = _converter.ConvertBack(converted, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalValue, convertedBack);
    }

    [TestMethod]
    public void ConvertBack_AndConvert_AreSymmetric_ForIndexZero()
    {
        // Arrange
        int originalIndex = 0;

        // Act
        var convertedBack = _converter.ConvertBack(originalIndex, typeof(int), null, CultureInfo.InvariantCulture);
        var converted = _converter.Convert(convertedBack, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalIndex, converted);
    }

    [TestMethod]
    public void ConvertBack_AndConvert_AreSymmetric_ForIndexFive()
    {
        // Arrange
        int originalIndex = 5;

        // Act
        var convertedBack = _converter.ConvertBack(originalIndex, typeof(int), null, CultureInfo.InvariantCulture);
        var converted = _converter.Convert(convertedBack, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(originalIndex, converted);
    }

    #endregion

    #region Edge Cases Tests

    [TestMethod]
    public void Convert_WithMaxInt_WorksCorrectly()
    {
        // Arrange
        int value = int.MaxValue;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(int.MaxValue - 1, result);
    }

    [TestMethod]
    public void ConvertBack_WithMaxIntMinusOne_WorksCorrectly()
    {
        // Arrange
        int value = int.MaxValue - 1;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(int.MaxValue, result);
    }

    [TestMethod]
    public void Convert_WithMinInt_ReturnsZero()
    {
        // Arrange
        int value = int.MinValue;

        // Act
        var result = _converter.Convert(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<int>(result);
        Assert.AreEqual(0, result);
    }

    #endregion

    #region Boundary Tests

    [TestMethod]
    public void Convert_BoundaryBetweenZeroAndOne_ReturnsZero()
    {
        // Arrange & Act & Assert
        Assert.AreEqual(0, _converter.Convert(0, typeof(int), null, CultureInfo.InvariantCulture));
        Assert.AreEqual(0, _converter.Convert(1, typeof(int), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertBack_BoundaryBetweenNegativeAndZero_ReturnsCorrectly()
    {
        // Arrange & Act & Assert
        Assert.AreEqual(1, _converter.ConvertBack(-1, typeof(int), null, CultureInfo.InvariantCulture));
        Assert.AreEqual(1, _converter.ConvertBack(0, typeof(int), null, CultureInfo.InvariantCulture));
    }

    #endregion

    #region Real-World Scenario Tests

    [TestMethod]
    public void Convert_TypicalListScenario_FirstItem()
    {
        // Arrange - Item ID 1 should map to index 0
        int itemId = 1;

        // Act
        var index = _converter.Convert(itemId, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(0, index, "First item (ID 1) should map to index 0");
    }

    [TestMethod]
    public void Convert_TypicalListScenario_LastItemOfTen()
    {
        // Arrange - Item ID 10 should map to index 9
        int itemId = 10;

        // Act
        var index = _converter.Convert(itemId, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(9, index, "Tenth item (ID 10) should map to index 9");
    }

    [TestMethod]
    public void ConvertBack_TypicalListScenario_FirstIndex()
    {
        // Arrange - Index 0 should map to item ID 1
        int index = 0;

        // Act
        var itemId = _converter.ConvertBack(index, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(1, itemId, "Index 0 should map to item ID 1");
    }

    [TestMethod]
    public void ConvertBack_TypicalListScenario_LastIndexOfTen()
    {
        // Arrange - Index 9 should map to item ID 10
        int index = 9;

        // Act
        var itemId = _converter.ConvertBack(index, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.AreEqual(10, itemId, "Index 9 should map to item ID 10");
    }

    #endregion
}
