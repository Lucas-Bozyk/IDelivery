namespace IDelivery.Domain.Interfaces.IRepositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
