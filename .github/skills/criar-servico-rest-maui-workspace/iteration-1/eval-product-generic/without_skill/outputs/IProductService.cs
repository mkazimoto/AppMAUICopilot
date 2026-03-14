using CameraApp.Models;

namespace CameraApp.Services;

/// <summary>
/// Defines standard CRUD operations for <see cref="Product" /> entities via the API.
/// </summary>
public interface IProductService : IBaseService<Product>
{
    // Standard CRUD operations are inherited from IBaseService<Product>.
    // Add product-specific methods here if needed in future iterations.
}
