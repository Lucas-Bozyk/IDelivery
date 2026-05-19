using IDelivery.Domain;

namespace IDelivery.Domain.Interfaces.IRepositories;

public interface IOrderRepository
{
    Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}
