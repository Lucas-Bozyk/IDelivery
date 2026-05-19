using IDelivery.Application.DTOs.Products;

namespace IDelivery.Application.IServices;

public interface IProductService
{
    Task<List<ProductDto>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
}
