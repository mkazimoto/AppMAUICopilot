using CameraApp.Config;
using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Provides standard CRUD operations for <see cref="Product" /> entities against the products API endpoint.
/// </summary>
public class ProductService : BaseService<Product>, IProductService
{
    /// <summary>
    /// Gets the relative API endpoint path for products.
    /// </summary>
    protected override string EndpointPath => ApiConfig.Endpoints.Products;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the API.</param>
    /// <param name="authService">The authentication service for token management.</param>
    public ProductService(HttpClient httpClient, IAuthService authService)
        : base(httpClient, authService)
    {
    }
}
