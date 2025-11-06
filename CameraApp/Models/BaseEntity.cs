namespace CameraApp.Models;

/// <summary>
/// Classe base para todas as entidades do sistema.
/// Fornece propriedades comuns como Id e campos de auditoria.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime? RecCreatedOn { get; set; }

    /// <summary>
    /// Usuário que criou o registro
    /// </summary>
    public string? RecCreatedBy { get; set; }

    /// <summary>
    /// Data da última modificação
    /// </summary>
    public DateTime? RecModifiedOn { get; set; }

    /// <summary>
    /// Usuário que modificou o registro
    /// </summary>
    public string? RecModifiedBy { get; set; }
}
