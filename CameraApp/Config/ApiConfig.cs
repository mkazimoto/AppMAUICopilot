namespace CameraApp.Config;

public static class ApiConfig
{
    // URL base da API TOTVS
    public const string BaseUrl = "http://10.82.138.36:8051";
    
    // Endpoints específicos
    public static class Endpoints
    {
        public const string Auth = "/api/connect/token";
        public const string Forms = "/api/construction-projects/v1/forms";

    }
    
    // Configurações de paginação
    public static class Pagination
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }
    
    // Timeout para requisições HTTP
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);
}