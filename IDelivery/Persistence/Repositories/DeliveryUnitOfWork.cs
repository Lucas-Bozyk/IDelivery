using IDelivery.Domain.Interfaces.IRepositories;

namespace IDelivery.Persistence.Repositories;

public class DeliveryUnitOfWork(DeliveryDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
