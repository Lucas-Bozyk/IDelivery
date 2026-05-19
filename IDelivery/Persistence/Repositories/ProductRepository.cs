using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Persistence.Repositories;

public class ProductRepository(DeliveryDbContext db) : IProductRepository
{
    public Task<List<Product>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default) =>
        db.Products.Where(x => x.RestaurantId == restaurantId).ToListAsync(ct);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await db.Products.AddAsync(product, ct);

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Remove(product);
        return Task.CompletedTask;
    }
}
