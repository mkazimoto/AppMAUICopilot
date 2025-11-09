namespace CameraApp.Models;

/// <summary>
/// Classe para filtros de consulta de formulários.
/// Centraliza todos os critérios de filtro disponíveis.
/// </summary>
public class FormFilter
{
    /// <summary>
    /// Filtrar por ID da categoria
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filtrar por ID do status
    /// </summary>
    public int? StatusFormId { get; set; }

    /// <summary>
    /// Filtrar por data de criação inicial
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filtrar por data de criação final
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filtrar por título (busca parcial)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Filtrar por usuário criador
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Filtrar apenas formulários com script sequencial
    /// </summary>
    public bool? SequentialScript { get; set; }

    /// <summary>
    /// Pontuação mínima
    /// </summary>
    public int? MinScore { get; set; }

    /// <summary>
    /// Pontuação máxima
    /// </summary>
    public int? MaxScore { get; set; }

    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Campo para ordenação
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Direção da ordenação (true = ascendente, false = descendente)
    /// </summary>
    public bool OrderAscending { get; set; } = true;

    /// <summary>
    /// Verifica se há algum filtro ativo (além de paginação)
    /// </summary>
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
    /// Constrói a query string OData com base nos filtros ativos
    /// </summary>
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
    /// Constrói a query string completa incluindo filtros, ordenação e paginação
    /// </summary>
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
    /// Reseta todos os filtros mantendo apenas a configuração de paginação
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
    /// Cria um clone do filtro atual
    /// </summary>
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
    /// Cria um filtro para buscar por categoria
    /// </summary>
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
    /// Cria um filtro para buscar por status
    /// </summary>
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
    /// Cria um filtro para buscar por período
    /// </summary>
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
    /// Cria um filtro padrão sem filtros ativos
    /// </summary>
    public static FormFilter Default(int page = 1, int pageSize = 10)
    {
        return new FormFilter
        {
            Page = page,
            PageSize = pageSize
        };
    }
}
