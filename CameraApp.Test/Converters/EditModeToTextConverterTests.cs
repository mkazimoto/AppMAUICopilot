using System.Globalization;
using CameraApp.Converters;

namespace CameraApp.Test.Converters;

[TestClass]
public class EditModeToTextConverterTests
{
    private EditModeToTextConverter _converter = null!;

    [TestInitialize]
    public void Setup()
    {
        _converter = new EditModeToTextConverter();
    }

    #region Convert Tests - Boolean Values

    [TestMethod]
    public void Convert_WhenValueIsTrue_ReturnsUpdateText()
    {
        // Arrange
        bool value = true;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Atualizar Formulário", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsFalse_ReturnsCreateText()
    {
        // Arrange
        bool value = false;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Criar Formulário", result);
    }

    #endregion

    #region Convert Tests - Non-Boolean Values

    [TestMethod]
    public void Convert_WhenValueIsNull_ReturnsDefaultText()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsString_ReturnsDefaultText()
    {
        // Arrange
        string value = "some string";

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsInteger_ReturnsDefaultText()
    {
        // Arrange
        int value = 42;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsDouble_ReturnsDefaultText()
    {
        // Arrange
        double value = 3.14;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsDateTime_ReturnsDefaultText()
    {
        // Arrange
        DateTime value = DateTime.Now;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    [TestMethod]
    public void Convert_WhenValueIsObject_ReturnsDefaultText()
    {
        // Arrange
        var value = new { Name = "Test" };

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Salvar", result);
    }

    #endregion

    #region Convert Tests - Culture and Parameter

    [TestMethod]
    public void Convert_WithDifferentCulture_WorksCorrectly()
    {
        // Arrange
        bool value = true;
        var culture = new CultureInfo("pt-BR");

        // Act
        var result = _converter.Convert(value, typeof(string), null, culture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Atualizar Formulário", result);
    }

    [TestMethod]
    public void Convert_WithParameter_IgnoresParameter()
    {
        // Arrange
        bool value = true;
        object parameter = "some parameter";

        // Act
        var result = _converter.Convert(value, typeof(string), parameter, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Atualizar Formulário", result);
    }

    [TestMethod]
    public void Convert_WithTargetType_ReturnsCorrectString()
    {
        // Arrange
        bool value = false;

        // Act
        var result = _converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Criar Formulário", result);
    }

    #endregion

    #region Convert Tests - Edge Cases

    [TestMethod]
    public void Convert_WithBoxedBooleanTrue_ReturnsUpdateText()
    {
        // Arrange
        object value = true;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Atualizar Formulário", result);
    }

    [TestMethod]
    public void Convert_WithBoxedBooleanFalse_ReturnsCreateText()
    {
        // Arrange
        object value = false;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<string>(result);
        Assert.AreEqual("Criar Formulário", result);
    }

    #endregion

    #region ConvertBack Tests

    [TestMethod]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        string value = "Atualizar Formulário";

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertBack_WithNullValue_ThrowsNotImplementedException()
    {
        // Arrange
        object? value = null;

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture));
    }

    [TestMethod]
    public void ConvertBack_WithEmptyString_ThrowsNotImplementedException()
    {
        // Arrange
        string value = string.Empty;

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() =>
            _converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture));
    }

    #endregion

    #region Text Validation Tests

    [TestMethod]
    public void Convert_EditModeTrue_ReturnsExpectedPortugueseText()
    {
        // Arrange
        bool value = true;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        var text = result.ToString();
        Assert.IsTrue(text?.Contains("Atualizar"), "Text should contain 'Atualizar'");
        Assert.IsTrue(text?.Contains("Formulário"), "Text should contain 'Formulário'");
    }

    [TestMethod]
    public void Convert_EditModeFalse_ReturnsExpectedPortugueseText()
    {
        // Arrange
        bool value = false;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        var text = result.ToString();
        Assert.IsTrue(text?.Contains("Criar"), "Text should contain 'Criar'");
        Assert.IsTrue(text?.Contains("Formulário"), "Text should contain 'Formulário'");
    }

    [TestMethod]
    public void Convert_NonBooleanValue_ReturnsExpectedDefaultText()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsNotNull(result);
        var text = result.ToString();
        Assert.AreEqual("Salvar", text, "Default text should be 'Salvar'");
    }

    #endregion
}
