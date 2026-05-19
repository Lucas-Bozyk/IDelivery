namespace IDelivery.Application.DTOs.Products;

public record ProductDto(Guid Id, string Name, string Description, decimal Price, bool IsAvailable);
