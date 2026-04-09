namespace CameraApp.Services;

/// <summary>
/// Provides CRUD operations for <see cref="Models.Product"/> entities.
/// </summary>
public class ProductService : BaseService<Models.Product>, IProductService
{
    /// <inheritdoc/>
    protected override string EndpointPath => ApiConfig.Endpoints.Products;

    /// <summary>
    /// Initializes a new instance of <see cref="ProductService"/>.
    /// </summary>
    public ProductService(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService) { }

    // Adicione métodos específicos aqui se necessário
}
