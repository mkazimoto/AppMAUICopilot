using System.Text.Json.Serialization;

namespace CameraApp.Models;

/// <summary>
/// Represents an employee entity in the system.
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>
    /// Gets or sets the full name of the employee.
    /// </summary>
    /// <value>The employee's full name. The default is an empty string.</value>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's job title or role.
    /// </summary>
    /// <value>The job title. The default is an empty string.</value>
    [JsonPropertyName("jobTitle")]
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department the employee belongs to.
    /// </summary>
    /// <value>The department name. The default is an empty string.</value>
    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's corporate e-mail address.
    /// </summary>
    /// <value>The e-mail address, or <see langword="null" /> if not set.</value>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the employee is currently active.
    /// </summary>
    /// <value><see langword="true" /> if the employee is active; otherwise, <see langword="false" />. The default is <see langword="true" />.</value>
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}
