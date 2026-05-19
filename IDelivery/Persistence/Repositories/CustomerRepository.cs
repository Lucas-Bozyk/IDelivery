using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Persistence.Repositories;

public class CustomerRepository(DeliveryDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        db.Customers.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public Task<List<CustomerAddress>> GetAddressesAsync(Guid customerId, CancellationToken ct = default) =>
        db.CustomerAddresses.Where(x => x.CustomerId == customerId).ToListAsync(ct);

    public async Task AddAddressAsync(CustomerAddress address, CancellationToken ct = default) =>
        await db.CustomerAddresses.AddAsync(address, ct);
}
