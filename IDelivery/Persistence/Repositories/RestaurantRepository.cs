using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Persistence.Repositories;

public class RestaurantRepository(DeliveryDbContext db) : IRestaurantRepository
{
    public Task<List<Restaurant>> GetAllAsync(CancellationToken ct = default) => db.Restaurants.ToListAsync(ct);

    public Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Restaurants.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Restaurant restaurant, CancellationToken ct = default) =>
        await db.Restaurants.AddAsync(restaurant, ct);

    public Task UpdateAsync(Restaurant restaurant, CancellationToken ct = default)
    {
        db.Restaurants.Update(restaurant);
        return Task.CompletedTask;
    }
}
