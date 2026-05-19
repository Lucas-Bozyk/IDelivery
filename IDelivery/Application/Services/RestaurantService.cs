using AutoMapper;
using IDelivery.Application.DTOs.Restaurants;
using IDelivery.Application.IServices;
using IDelivery.Domain.Interfaces.IRepositories;

namespace IDelivery.Application.Services;

public class RestaurantService(IRestaurantRepository restaurants, IMapper mapper) : IRestaurantService
{
    public async Task<List<RestaurantDto>> GetAllAsync(CancellationToken ct = default) =>
        mapper.Map<List<RestaurantDto>>(await restaurants.GetAllAsync(ct));

    public async Task<RestaurantDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var r = await restaurants.GetByIdAsync(id, ct);
        return r is null ? null : mapper.Map<RestaurantDto>(r);
    }
}
