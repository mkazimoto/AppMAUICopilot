using System.Text.Json;

namespace CameraApp.Config;

/// <summary>
/// Contains static API configuration constants, endpoint paths, and serialization settings.
/// </summary>
public static class ApiConfig
{
    /// <summary>The base URL of the TOTVS API server.</summary>
    public const string BaseUrl = "http://192.168.1.11:8051";

    /// <summary>
    /// Provides the relative path constants for each API endpoint.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>The relative path to the OAuth 2.0 token endpoint.</summary>
        public const string Auth = "/api/connect/token";
        /// <summary>The relative path to the forms resource endpoint.</summary>
        public const string Forms = "/api/construction-projects/v1/forms";
    }

    /// <summary>
    /// Provides pagination-related configuration constants.
    /// </summary>
    public static class Pagination
    {
        /// <summary>The default number of items returned per page.</summary>
        public const int DefaultPageSize = 10;
        /// <summary>The maximum number of items that can be requested per page.</summary>
        public const int MaxPageSize = 100;
    }

    /// <summary>Gets the default timeout for HTTP requests.</summary>
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Returns a <see cref="JsonSerializerOptions" /> instance configured for case-insensitive camelCase JSON handling.
    /// </summary>
    /// <returns>A new <see cref="JsonSerializerOptions" /> with <c>PropertyNameCaseInsensitive</c> and camelCase naming policy enabled.</returns>
    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}