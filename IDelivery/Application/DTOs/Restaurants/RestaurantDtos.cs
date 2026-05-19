namespace IDelivery.Application.DTOs.Restaurants;

public record RestaurantDto(Guid Id, string Name, string Description, bool IsOpen);
