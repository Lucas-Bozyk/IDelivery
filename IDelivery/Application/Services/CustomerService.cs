using AutoMapper;
using IDelivery.Application.DTOs.Customers;
using IDelivery.Application.IServices;
using IDelivery.Domain.Interfaces.IRepositories;

namespace IDelivery.Application.Services;

public class CustomerService(ICustomerRepository customers, IMapper mapper) : ICustomerService
{
    public async Task<CustomerMeDto?> GetMeAsync(Guid userId, CancellationToken ct = default)
    {
        var customer = await customers.GetByUserIdAsync(userId, ct);
        return customer is null ? null : mapper.Map<CustomerMeDto>(customer);
    }

    public async Task<List<AddressDto>> GetAddressesAsync(Guid userId, CancellationToken ct = default)
    {
        var customer = await customers.GetByUserIdAsync(userId, ct);
        if (customer is null) return [];
        var addresses = await customers.GetAddressesAsync(customer.Id, ct);
        return mapper.Map<List<AddressDto>>(addresses);
    }
}
