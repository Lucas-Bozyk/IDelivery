using IDelivery.Application.DTOs.Orders;
using IDelivery.Domain;

namespace IDelivery.Application.IServices;

public interface IOrderService
{
    Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default);
}
