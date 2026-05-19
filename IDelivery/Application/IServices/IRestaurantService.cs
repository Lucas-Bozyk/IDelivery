using IDelivery.Application.DTOs.Restaurants;

namespace IDelivery.Application.IServices;

public interface IRestaurantService
{
    Task<List<RestaurantDto>> GetAllAsync(CancellationToken ct = default);
    Task<RestaurantDto?> GetAsync(Guid id, CancellationToken ct = default);
}
