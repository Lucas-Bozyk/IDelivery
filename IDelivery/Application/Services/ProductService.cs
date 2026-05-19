using AutoMapper;
using IDelivery.Application.DTOs.Products;
using IDelivery.Application.IServices;
using IDelivery.Domain.Interfaces.IRepositories;

namespace IDelivery.Application.Services;

public class ProductService(IProductRepository products, IMapper mapper) : IProductService
{
    public async Task<List<ProductDto>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default) =>
        mapper.Map<List<ProductDto>>(await products.GetByRestaurantAsync(restaurantId, ct));
}
