using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Persistence.Repositories;

public class OrderRepository(DeliveryDbContext db) : IOrderRepository
{
    public Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        db.Orders.Where(x => x.CustomerId == customerId).ToListAsync(ct);

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Update(order);
        return Task.CompletedTask;
    }
}
