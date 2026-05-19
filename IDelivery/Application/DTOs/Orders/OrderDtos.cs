using IDelivery.Domain;

namespace IDelivery.Application.DTOs.Orders;

public record OrderDto(Guid Id, OrderStatus Status, decimal Total);
