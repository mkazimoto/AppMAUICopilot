using CameraApp.Models;
using CameraApp.Services;
using CameraApp.ViewModels;

namespace CameraApp.Test.ViewModels;

public class FormEditViewModelTests
{
    private readonly Mock<IFormService> _formServiceMock;

    public FormEditViewModelTests()
    {
        _formServiceMock = new Mock<IFormService>();
    }

    private FormEditViewModel CreateSut() =>
        new(_formServiceMock.Object);

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_InitializesFourCategories()
    {
        var sut = CreateSut();
        Assert.Equal(4, sut.Categories.Count);
    }

    [Fact]
    public void Constructor_InitializesThreeStatusItems()
    {
        var sut = CreateSut();
        Assert.Equal(3, sut.StatusItems.Count);
    }

    [Fact]
    public void Constructor_IsloAdingIsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.IsLoading);
    }

    // ── SetCreateMode ────────────────────────────────────────────────────────

    [Fact]
    public void SetCreateMode_IsEditModeIsFalse()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.False(sut.IsEditMode);
    }

    [Fact]
    public void SetCreateMode_CanDeleteIsFalse()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.False(sut.CanDelete);
    }

    [Fact]
    public void SetCreateMode_PageTitleIsNovoFormulario()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.Equal("Novo Formulário", sut.PageTitle);
    }

    [Fact]
    public void SetCreateMode_TitleIsEmpty()
    {
        var sut = CreateSut();
        sut.Title = "Some title";
        sut.SetCreateMode();
        Assert.Equal(string.Empty, sut.Title);
    }

    [Fact]
    public void SetCreateMode_FormIdIsEmpty()
    {
        var sut = CreateSut();
        sut.FormId = "existing-id";
        sut.SetCreateMode();
        Assert.Equal(string.Empty, sut.FormId);
    }

    [Fact]
    public void SetCreateMode_SequentialScriptIsFalse()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.False(sut.SequentialScript);
    }

    [Fact]
    public void SetCreateMode_TotalScoreIsZero()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.Equal(0, sut.TotalScore);
    }

    [Fact]
    public void SetCreateMode_CategoryItemIsFirstCategory()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.NotNull(sut.CategoryItem);
        Assert.Equal(1, sut.CategoryItem.Id); // first category has Id = 1
    }

    [Fact]
    public void SetCreateMode_StatusItemIsFirstStatus()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        Assert.NotNull(sut.StatusItem);
        Assert.Equal(0, sut.StatusItem.Id); // first status (Rascunho) has Id = 0
    }

    // ── SetEditMode ──────────────────────────────────────────────────────────

    [Fact]
    public void SetEditMode_IsEditModeIsTrue()
    {
        var sut = CreateSut();
        var form = MakeForm();
        sut.SetEditMode(form);
        Assert.True(sut.IsEditMode);
    }

    [Fact]
    public void SetEditMode_CanDeleteIsTrue()
    {
        var sut = CreateSut();
        sut.SetEditMode(MakeForm());
        Assert.True(sut.CanDelete);
    }

    [Fact]
    public void SetEditMode_PageTitleIsEditarFormulario()
    {
        var sut = CreateSut();
        sut.SetEditMode(MakeForm());
        Assert.Equal("Editar Formulário", sut.PageTitle);
    }

    [Fact]
    public void SetEditMode_LoadsFormId()
    {
        var sut = CreateSut();
        var form = MakeForm();
        sut.SetEditMode(form);
        Assert.Equal("form-123", sut.FormId);
    }

    [Fact]
    public void SetEditMode_LoadsTitle()
    {
        var sut = CreateSut();
        sut.SetEditMode(MakeForm());
        Assert.Equal("Test Form", sut.Title);
    }

    [Fact]
    public void SetEditMode_LoadsCategoryItem()
    {
        var sut = CreateSut();
        var form = MakeForm(categoryId: 2);
        sut.SetEditMode(form);
        Assert.Equal(2, sut.CategoryItem.Id);
    }

    [Fact]
    public void SetEditMode_LoadsStatusItem()
    {
        var sut = CreateSut();
        var form = MakeForm(statusFormId: 1);
        sut.SetEditMode(form);
        Assert.Equal(1, sut.StatusItem.Id);
    }

    [Fact]
    public void SetEditMode_LoadsSequentialScript()
    {
        var sut = CreateSut();
        var form = MakeForm(sequentialScript: true);
        sut.SetEditMode(form);
        Assert.True(sut.SequentialScript);
    }

    [Fact]
    public void SetEditMode_LoadsTotalScore()
    {
        var sut = CreateSut();
        var form = MakeForm(totalScore: 75);
        sut.SetEditMode(form);
        Assert.Equal(75, sut.TotalScore);
    }

    // ── SaveAsync – validation guard ─────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_WhenIsLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = "Valid Title";
        sut.IsLoading = true;

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.CreateFormAsync(It.IsAny<Form>()), Times.Never);
        _formServiceMock.Verify(s => s.UpdateFormAsync(It.IsAny<string>(), It.IsAny<Form>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_WhenTitleIsEmpty_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = string.Empty;

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.CreateFormAsync(It.IsAny<Form>()), Times.Never);
    }

    // ── SaveAsync – create mode ──────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_CreateMode_CallsCreateFormAsync()
    {
        // Return null so no Shell.Current.GoToAsync is called
        _formServiceMock
            .Setup(s => s.CreateFormAsync(It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = "New Form";

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.CreateFormAsync(It.IsAny<Form>()), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_CreateMode_PassesTitleToService()
    {
        _formServiceMock
            .Setup(s => s.CreateFormAsync(It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = "Form Title";

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(
            s => s.CreateFormAsync(It.Is<Form>(f => f.Title == "Form Title")),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_CreateMode_SetsIsLoadingFalseWhenComplete()
    {
        _formServiceMock
            .Setup(s => s.CreateFormAsync(It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = "New Form";

        await sut.SaveCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task SaveAsync_CreateMode_SetsIsLoadingFalseOnException()
    {
        _formServiceMock
            .Setup(s => s.CreateFormAsync(It.IsAny<Form>()))
            .ThrowsAsync(new HttpRequestException("Server error"));

        var sut = CreateSut();
        sut.SetCreateMode();
        sut.Title = "New Form";

        await sut.SaveCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    // ── SaveAsync – edit mode ────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_EditMode_CallsUpdateFormAsync()
    {
        // Return null so no Shell.Current.GoToAsync is called
        _formServiceMock
            .Setup(s => s.UpdateFormAsync(It.IsAny<string>(), It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetEditMode(MakeForm());
        sut.Title = "Updated Title";

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(
            s => s.UpdateFormAsync("form-123", It.IsAny<Form>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_EditMode_DoesNotCallCreateFormAsync()
    {
        _formServiceMock
            .Setup(s => s.UpdateFormAsync(It.IsAny<string>(), It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetEditMode(MakeForm());

        await sut.SaveCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.CreateFormAsync(It.IsAny<Form>()), Times.Never);
    }

    [Fact]
    public async Task SaveAsync_EditMode_SetsIsLoadingFalseWhenComplete()
    {
        _formServiceMock
            .Setup(s => s.UpdateFormAsync(It.IsAny<string>(), It.IsAny<Form>()))
            .ReturnsAsync((Form?)null);

        var sut = CreateSut();
        sut.SetEditMode(MakeForm());

        await sut.SaveCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenIsLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.SetEditMode(MakeForm());
        sut.IsLoading = true;

        await sut.DeleteCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.DeleteFormAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotEditMode_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.SetCreateMode();

        await sut.DeleteCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.DeleteFormAsync(It.IsAny<string>()), Times.Never);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Form MakeForm(
        string id = "form-123",
        string title = "Test Form",
        int categoryId = 1,
        int statusFormId = 0,
        bool sequentialScript = false,
        int totalScore = 50)
    {
        return new Form
        {
            Id = id,
            Title = title,
            CategoryId = categoryId,
            StatusFormId = statusFormId,
            SequentialScript = sequentialScript,
            TotalScore = totalScore,
        };
    }
}
