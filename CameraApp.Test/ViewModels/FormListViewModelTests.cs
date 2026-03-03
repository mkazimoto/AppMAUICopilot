using CameraApp.Models;
using CameraApp.Services;
using CameraApp.ViewModels;

namespace CameraApp.Test.ViewModels;

public class FormListViewModelTests
{
    private readonly Mock<IFormService> _formServiceMock;
    private readonly Mock<IAuthService> _authServiceMock;

    public FormListViewModelTests()
    {
        _formServiceMock = new Mock<IFormService>();
        _authServiceMock = new Mock<IAuthService>();

        // Default: return an empty FormResponse
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = new List<Form>(), HasNext = false });
    }

    private FormListViewModel CreateSut() =>
        new(_formServiceMock.Object, _authServiceMock.Object);

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_InitializesFiveCategories()
    {
        var sut = CreateSut();
        Assert.Equal(5, sut.Categories.Count);
    }

    [Fact]
    public void Constructor_InitializesFourStatusItems()
    {
        var sut = CreateSut();
        Assert.Equal(4, sut.StatusItems.Count);
    }

    // ── Initial state ────────────────────────────────────────────────────────

    [Fact]
    public void InitialState_IsLoading_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public void InitialState_CurrentPage_IsOne()
    {
        var sut = CreateSut();
        Assert.Equal(1, sut.CurrentPage);
    }

    [Fact]
    public void InitialState_Forms_IsEmpty()
    {
        var sut = CreateSut();
        Assert.Empty(sut.Forms);
    }

    [Fact]
    public void InitialState_HasActiveFilters_IsFalse()
    {
        var sut = CreateSut();
        Assert.False(sut.HasActiveFilters);
    }

    // ── LoadFormsAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task LoadFormsAsync_WhenAlreadyLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.IsLoading = true;

        await sut.LoadFormsCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.GetFormsAsync(It.IsAny<FormFilter>()), Times.Never);
    }

    [Fact]
    public async Task LoadFormsAsync_PopulatesFormsCollection()
    {
        var forms = new List<Form>
        {
            new Form { Id = "1", Title = "Form A" },
            new Form { Id = "2", Title = "Form B" },
        };

        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = forms, HasNext = false });

        var sut = CreateSut();
        await sut.LoadFormsCommand.ExecuteAsync(null);

        Assert.Equal(2, sut.Forms.Count);
        Assert.Equal("Form A", sut.Forms[0].Title);
        Assert.Equal("Form B", sut.Forms[1].Title);
    }

    [Fact]
    public async Task LoadFormsAsync_SetsHasNextPage_WhenResponseHasNext()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = new List<Form>(), HasNext = true });

        var sut = CreateSut();
        await sut.LoadFormsCommand.ExecuteAsync(null);

        Assert.True(sut.HasNextPage);
    }

    [Fact]
    public async Task LoadFormsAsync_SetsIsLoadingFalse_WhenComplete()
    {
        var sut = CreateSut();
        await sut.LoadFormsCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoadFormsAsync_SetsIsLoadingFalse_WhenServiceThrows()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ThrowsAsync(new HttpRequestException("Timeout"));

        var sut = CreateSut();
        await sut.LoadFormsCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task LoadFormsAsync_ResetsToPageOne()
    {
        var sut = CreateSut();
        sut.CurrentPage = 5;

        await sut.LoadFormsCommand.ExecuteAsync(null);

        Assert.Equal(1, sut.CurrentPage);
    }

    [Fact]
    public async Task LoadFormsAsync_ClearsExistingForms()
    {
        _formServiceMock
            .SetupSequence(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = new List<Form> { new Form { Id = "1", Title = "Old" } }, HasNext = false })
            .ReturnsAsync(new FormResponse { Items = new List<Form> { new Form { Id = "2", Title = "New" } }, HasNext = false });

        var sut = CreateSut();
        await sut.LoadFormsCommand.ExecuteAsync(null); // loads "Old"
        await sut.LoadFormsCommand.ExecuteAsync(null); // reloads with "New"

        Assert.Single(sut.Forms);
        Assert.Equal("New", sut.Forms[0].Title);
    }

    // ── LoadMoreFormsAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task LoadMoreFormsAsync_WhenHasNextPageFalse_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.HasNextPage = false;

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.GetFormsAsync(It.IsAny<FormFilter>()), Times.Never);
    }

    [Fact]
    public async Task LoadMoreFormsAsync_WhenIsLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.HasNextPage = true;
        sut.IsLoading = true;

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.GetFormsAsync(It.IsAny<FormFilter>()), Times.Never);
    }

    [Fact]
    public async Task LoadMoreFormsAsync_AppendsForms()
    {
        var page2Forms = new List<Form>
        {
            new Form { Id = "3", Title = "Form C" },
            new Form { Id = "4", Title = "Form D" },
        };

        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.Is<FormFilter>(f => f.Page == 2)))
            .ReturnsAsync(new FormResponse { Items = page2Forms, HasNext = false });

        var sut = CreateSut();
        sut.HasNextPage = true;
        sut.Forms.Add(new Form { Id = "1", Title = "Form A" });
        sut.Forms.Add(new Form { Id = "2", Title = "Form B" });

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        Assert.Equal(4, sut.Forms.Count);
        Assert.Equal("Form C", sut.Forms[2].Title);
    }

    [Fact]
    public async Task LoadMoreFormsAsync_IncrementsCurrentPage()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = new List<Form>(), HasNext = false });

        var sut = CreateSut();
        sut.HasNextPage = true;

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        Assert.Equal(2, sut.CurrentPage);
    }

    [Fact]
    public async Task LoadMoreFormsAsync_OnException_RevertsCurrentPage()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ThrowsAsync(new HttpRequestException("Error"));

        var sut = CreateSut();
        sut.HasNextPage = true;
        var originalPage = sut.CurrentPage;

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        Assert.Equal(originalPage, sut.CurrentPage);
    }

    [Fact]
    public async Task LoadMoreFormsAsync_SetsIsLoadingFalse_WhenComplete()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = new List<Form>(), HasNext = false });

        var sut = CreateSut();
        sut.HasNextPage = true;

        await sut.LoadMoreFormsCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    // ── SearchByTitleAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task SearchByTitleAsync_WhenIsLoading_DoesNotCallService()
    {
        var sut = CreateSut();
        sut.IsLoading = true;
        sut.SearchTitle = "test";

        await sut.SearchByTitleCommand.ExecuteAsync(null);

        _formServiceMock.Verify(s => s.GetFormsAsync(It.IsAny<FormFilter>()), Times.Never);
    }

    [Fact]
    public async Task SearchByTitleAsync_PassesTitleToFilter()
    {
        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.Is<FormFilter>(f => f.Title == "MyForm")))
            .ReturnsAsync(new FormResponse { Items = new List<Form>(), HasNext = false });

        var sut = CreateSut();
        sut.SearchTitle = "MyForm";

        await sut.SearchByTitleCommand.ExecuteAsync(null);

        _formServiceMock.Verify(
            s => s.GetFormsAsync(It.Is<FormFilter>(f => f.Title == "MyForm")),
            Times.Once);
    }

    [Fact]
    public async Task SearchByTitleAsync_PopulatesForms()
    {
        var results = new List<Form> { new Form { Id = "1", Title = "MyForm" } };

        _formServiceMock
            .Setup(s => s.GetFormsAsync(It.IsAny<FormFilter>()))
            .ReturnsAsync(new FormResponse { Items = results, HasNext = false });

        var sut = CreateSut();
        sut.SearchTitle = "MyForm";

        await sut.SearchByTitleCommand.ExecuteAsync(null);

        Assert.Single(sut.Forms);
        Assert.Equal("MyForm", sut.Forms[0].Title);
    }

    [Fact]
    public async Task SearchByTitleAsync_SetsIsLoadingFalse_WhenComplete()
    {
        var sut = CreateSut();
        await sut.SearchByTitleCommand.ExecuteAsync(null);

        Assert.False(sut.IsLoading);
    }

    // ── ClearAdvancedFilters ─────────────────────────────────────────────────

    [Fact]
    public void ClearAdvancedFilters_ResetsSelections()
    {
        var sut = CreateSut();
        sut.SelectedCategoryItem = new CategoryItem { Id = 1, Name = "Tarefa" };
        sut.SelectedStatusItem = new StatusItem { Id = 1, Name = "Publicado" };

        sut.ClearAdvancedFiltersCommand.Execute(null);

        Assert.Null(sut.SelectedCategoryItem);
        Assert.Null(sut.SelectedStatusItem);
    }

    [Fact]
    public void ClearAdvancedFilters_ResetsTextFields()
    {
        var sut = CreateSut();
        sut.CreatedBy = "someone";

        sut.ClearAdvancedFiltersCommand.Execute(null);

        Assert.Equal(string.Empty, sut.CreatedBy);
    }

    [Fact]
    public void ClearAdvancedFilters_ResetsBoolFields()
    {
        var sut = CreateSut();
        sut.FilterSequentialScript = true;
        sut.OrderAscending = false;

        sut.ClearAdvancedFiltersCommand.Execute(null);

        Assert.False(sut.FilterSequentialScript);
        Assert.True(sut.OrderAscending);
    }

    [Fact]
    public void ClearAdvancedFilters_ResetsNullableScores()
    {
        var sut = CreateSut();
        sut.MinScore = 10;
        sut.MaxScore = 100;

        sut.ClearAdvancedFiltersCommand.Execute(null);

        Assert.Null(sut.MinScore);
        Assert.Null(sut.MaxScore);
    }

    [Fact]
    public void ClearAdvancedFilters_ResetsOrderByIndex()
    {
        var sut = CreateSut();
        sut.OrderByIndex = 3;

        sut.ClearAdvancedFiltersCommand.Execute(null);

        Assert.Equal(0, sut.OrderByIndex);
    }
}
