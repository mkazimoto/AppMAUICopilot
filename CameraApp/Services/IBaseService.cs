using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Interface base genérica para serviços REST que operam sobre entidades.
/// Define operações CRUD padrão.
/// </summary>
/// <typeparam name="T">Tipo da entidade que herda de BaseEntity</typeparam>
public interface IBaseService<T> where T : BaseEntity
{
    /// <summary>
    /// Obtém uma lista paginada de entidades
    /// </summary>
    /// <param name="page">Número da página (começa em 1)</param>
    /// <param name="pageSize">Quantidade de itens por página</param>
    /// <returns>Response contendo a lista de entidades e informações de paginação</returns>
    Task<PaginatedResponse<T>> GetAllAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Obtém uma entidade por ID
    /// </summary>
    /// <param name="id">Identificador da entidade</param>
    /// <returns>A entidade encontrada ou null se não existir</returns>
    Task<T?> GetByIdAsync(string id);

    /// <summary>
    /// Cria uma nova entidade
    /// </summary>
    /// <param name="entity">Entidade a ser criada</param>
    /// <returns>A entidade criada com dados atualizados da API</returns>
    Task<T?> CreateAsync(T entity);

    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    /// <param name="id">Identificador da entidade</param>
    /// <param name="entity">Entidade com dados atualizados</param>
    /// <returns>A entidade atualizada ou null se houve erro</returns>
    Task<T?> UpdateAsync(string id, T entity);

    /// <summary>
    /// Exclui uma entidade
    /// </summary>
    /// <param name="id">Identificador da entidade</param>
    /// <returns>True se a exclusão foi bem-sucedida, false caso contrário</returns>
    Task<bool> DeleteAsync(string id);
}

/// <summary>
/// Classe genérica para resposta paginada da API
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public class PaginatedResponse<T> where T : BaseEntity
{
    /// <summary>
    /// Lista de itens da página atual
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Página atual
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens disponíveis
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas disponíveis
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Indica se há página anterior
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indica se há próxima página
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
