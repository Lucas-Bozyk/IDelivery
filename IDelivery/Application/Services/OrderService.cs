using AutoMapper;
using IDelivery.Application.DTOs.Orders;
using IDelivery.Application.IServices;
using IDelivery.Application.UseCases;
using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;

namespace IDelivery.Application.Services;

public class OrderService(IOrderRepository orders, UpdateOrderStatusUseCase updateOrderStatusUseCase, IMapper mapper) : IOrderService
{
    public async Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        mapper.Map<List<OrderDto>>(await orders.GetByCustomerAsync(customerId, ct));

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var o = await orders.GetByIdAsync(id, ct);
        return o is null ? null : mapper.Map<OrderDto>(o);
    }

    public async Task UpdateStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default)
    {
        await updateOrderStatusUseCase.ExecuteAsync(new UpdateOrderStatusCommand(orderId, status), ct);
    }
}
