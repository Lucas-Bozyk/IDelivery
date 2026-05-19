using IDelivery.Domain;

namespace IDelivery.Domain.Interfaces.IRepositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<CustomerAddress>> GetAddressesAsync(Guid customerId, CancellationToken ct = default);
    Task AddAddressAsync(CustomerAddress address, CancellationToken ct = default);
}
