namespace CameraApp.Models;

/// <summary>
/// Encapsulates all filter and pagination criteria for querying forms.
/// </summary>
public class FormFilter
{
    /// <summary>
    /// Gets or sets the category identifier to filter by.
    /// </summary>
    /// <value>The category ID, or <see langword="null" /> to include all categories.</value>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the status identifier to filter by.
    /// </summary>
    /// <value>The status ID, or <see langword="null" /> to include all statuses.</value>
    public int? StatusFormId { get; set; }

    /// <summary>
    /// Gets or sets the earliest creation date to include in the results.
    /// </summary>
    /// <value>The start of the creation date range, or <see langword="null" /> for no lower bound.</value>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the latest creation date to include in the results.
    /// </summary>
    /// <value>The end of the creation date range, or <see langword="null" /> for no upper bound.</value>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the partial title text to search for.
    /// </summary>
    /// <value>The title substring, or <see langword="null" /> to skip title filtering.</value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the username of the creator to filter by.
    /// </summary>
    /// <value>The creator username, or <see langword="null" /> to include all creators.</value>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets a value that filters forms by their sequential script setting.
    /// </summary>
    /// <value><see langword="true" /> to return only sequential-script forms; <see langword="false" /> for non-sequential; <see langword="null" /> to include both.</value>
    public bool? SequentialScript { get; set; }

    /// <summary>
    /// Gets or sets the minimum total score a form must have to be included.
    /// </summary>
    /// <value>The minimum score threshold, or <see langword="null" /> for no lower bound.</value>
    public int? MinScore { get; set; }

    /// <summary>
    /// Gets or sets the maximum total score a form may have to be included.
    /// </summary>
    /// <value>The maximum score threshold, or <see langword="null" /> for no upper bound.</value>
    public int? MaxScore { get; set; }

    /// <summary>
    /// Gets or sets the one-based page number to retrieve.
    /// </summary>
    /// <value>The page number. The default is <c>1</c>.</value>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    /// <value>The page size. The default is <c>10</c>.</value>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the field name used for sorting results.
    /// </summary>
    /// <value>The OData field name to sort by, or <see langword="null" /> for default ordering.</value>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether results are sorted in ascending order.
    /// </summary>
    /// <value><see langword="true" /> for ascending order; <see langword="false" /> for descending. The default is <see langword="true" />.</value>
    public bool OrderAscending { get; set; } = true;

    /// <summary>
    /// Gets a value that indicates whether any non-pagination filter is active.
    /// </summary>
    /// <value><see langword="true" /> if at least one filter criterion is set; otherwise, <see langword="false" />.</value>
    public bool HasFilters =>
        CategoryId.HasValue ||
        StatusFormId.HasValue ||
        StartDate.HasValue ||
        EndDate.HasValue ||
        !string.IsNullOrEmpty(Title) ||
        !string.IsNullOrEmpty(CreatedBy) ||
        SequentialScript.HasValue ||
        MinScore.HasValue ||
        MaxScore.HasValue;

    /// <summary>
    /// Builds an OData <c>$filter</c> expression based on the currently active filter properties.
    /// </summary>
    /// <returns>A URL-encoded OData filter string, or an empty string if no filters are active.</returns>
    public string BuildODataFilter()
    {
        var filters = new List<string>();

        if (CategoryId.HasValue)
            filters.Add($"categoryId eq {CategoryId.Value}");

        if (StatusFormId.HasValue)
            filters.Add($"statusFormId eq {StatusFormId.Value}");

        if (!string.IsNullOrEmpty(Title))
            filters.Add($"contains(title, '{Title}')");

        if (!string.IsNullOrEmpty(CreatedBy))
            filters.Add($"recCreatedBy eq '{CreatedBy}'");

        if (SequentialScript.HasValue)
            filters.Add($"sequentialScript eq {SequentialScript.Value.ToString().ToLower()}");

        if (MinScore.HasValue)
            filters.Add($"totalScore ge {MinScore.Value}");

        if (MaxScore.HasValue)
            filters.Add($"totalScore le {MaxScore.Value}");

        if (StartDate.HasValue)
        {
            var startDateStr = StartDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            filters.Add($"recCreatedOn ge {startDateStr}");
        }

        if (EndDate.HasValue)
        {
            var endDateStr = EndDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            filters.Add($"recCreatedOn le {endDateStr}");
        }

        return filters.Count > 0 ? string.Join(" and ", filters) : string.Empty;
    }

    /// <summary>
    /// Builds the full query string including OData filters, ordering, and pagination parameters.
    /// </summary>
    /// <returns>An ampersand-delimited query string ready to append to an API endpoint URL.</returns>
    public string BuildQueryString()
    {
        var queryParams = new List<string>();

        // Filtros OData
        var filterString = BuildODataFilter();
        if (!string.IsNullOrEmpty(filterString))
        {
            queryParams.Add($"$filter={Uri.EscapeDataString(filterString)}");
        }

        // Ordenação
        if (!string.IsNullOrEmpty(OrderBy))
        {
            var orderDirection = OrderAscending ? "asc" : "desc";
            queryParams.Add($"$orderby={OrderBy} {orderDirection}");
        }

        // Paginação
        queryParams.Add($"page={Page}");
        queryParams.Add($"pagesize={PageSize}");

        return string.Join("&", queryParams);
    }

    /// <summary>
    /// Resets all filter criteria to their default values while preserving the current pagination settings.
    /// </summary>
    public void Reset()
    {
        CategoryId = null;
        StatusFormId = null;
        StartDate = null;
        EndDate = null;
        Title = null;
        CreatedBy = null;
        SequentialScript = null;
        MinScore = null;
        MaxScore = null;
        OrderBy = null;
        OrderAscending = true;
    }

    /// <summary>
    /// Creates a shallow copy of the current filter.
    /// </summary>
    /// <returns>A new <see cref="FormFilter" /> instance with the same property values.</returns>
    public FormFilter Clone()
    {
        return new FormFilter
        {
            CategoryId = this.CategoryId,
            StatusFormId = this.StatusFormId,
            StartDate = this.StartDate,
            EndDate = this.EndDate,
            Title = this.Title,
            CreatedBy = this.CreatedBy,
            SequentialScript = this.SequentialScript,
            MinScore = this.MinScore,
            MaxScore = this.MaxScore,
            Page = this.Page,
            PageSize = this.PageSize,
            OrderBy = this.OrderBy,
            OrderAscending = this.OrderAscending
        };
    }

    /// <summary>
    /// Creates a filter that restricts results to a specific category.
    /// </summary>
    /// <param name="categoryId">The category identifier to filter by.</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="FormFilter" /> pre-configured for the specified category.</returns>
    public static FormFilter ByCategory(int categoryId, int page = 1, int pageSize = 10)
    {
        return new FormFilter
        {
            CategoryId = categoryId,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates a filter that restricts results to a specific status.
    /// </summary>
    /// <param name="statusId">The status identifier to filter by.</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="FormFilter" /> pre-configured for the specified status.</returns>
    public static FormFilter ByStatus(int statusId, int page = 1, int pageSize = 10)
    {
        return new FormFilter
        {
            StatusFormId = statusId,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates a filter that restricts results to a specific creation date range.
    /// </summary>
    /// <param name="startDate">The start of the date range (inclusive).</param>
    /// <param name="endDate">The end of the date range (inclusive).</param>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="FormFilter" /> pre-configured for the specified date range.</returns>
    public static FormFilter ByDateRange(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10)
    {
        return new FormFilter
        {
            StartDate = startDate,
            EndDate = endDate,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates a default filter with no active filter criteria.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="FormFilter" /> with only pagination configured.</returns>
    public static FormFilter Default(int page = 1, int pageSize = 10)
    {
        return new FormFilter
        {
            Page = page,
            PageSize = pageSize
        };
    }
}
