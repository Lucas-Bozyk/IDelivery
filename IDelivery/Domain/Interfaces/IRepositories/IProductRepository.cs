using IDelivery.Domain;

namespace IDelivery.Domain.Interfaces.IRepositories;

public interface IProductRepository
{
    Task<List<Product>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Product product, CancellationToken ct = default);
}
