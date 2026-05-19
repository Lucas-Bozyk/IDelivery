using IDelivery.Domain;

namespace IDelivery.Domain.Interfaces.IRepositories;

public interface IRestaurantRepository
{
    Task<List<Restaurant>> GetAllAsync(CancellationToken ct = default);
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Restaurant restaurant, CancellationToken ct = default);
    Task UpdateAsync(Restaurant restaurant, CancellationToken ct = default);
}
