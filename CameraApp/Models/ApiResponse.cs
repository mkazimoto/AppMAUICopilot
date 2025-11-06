using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Classe base genérica para respostas de API que retornam listas de itens.
/// Compatível com o formato de resposta da API TOTVS.
/// </summary>
/// <typeparam name="T">Tipo da entidade retornada na lista</typeparam>
public class ApiResponse<T> where T : BaseEntity
{
    /// <summary>
    /// Indica se há mais itens disponíveis (próxima página)
    /// </summary>
    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }

    /// <summary>
    /// Lista de itens retornados pela API
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Quantidade total de itens retornados
    /// </summary>
    [JsonIgnore]
    public int Count => Items?.Count ?? 0;

    /// <summary>
    /// Indica se a resposta está vazia (sem itens)
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty => Count == 0;
}

/// <summary>
/// Classe específica para respostas de formulários.
/// Mantida para compatibilidade com código existente.
/// </summary>
public class FormResponse : ApiResponse<Form>
{
}
