namespace IDelivery.Application.DTOs.Carts;

public record CartDto(Guid Id, Guid CustomerId, List<CartItemDto> Items);
public record CartItemDto(Guid Id, Guid ProductId, int Quantity);
