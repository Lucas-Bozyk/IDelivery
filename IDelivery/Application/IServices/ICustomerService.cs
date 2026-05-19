using IDelivery.Application.DTOs.Customers;

namespace IDelivery.Application.IServices;

public interface ICustomerService
{
    Task<CustomerMeDto?> GetMeAsync(Guid userId, CancellationToken ct = default);
    Task<List<AddressDto>> GetAddressesAsync(Guid userId, CancellationToken ct = default);
}
