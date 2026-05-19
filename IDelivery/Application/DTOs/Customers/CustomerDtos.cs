namespace IDelivery.Application.DTOs.Customers;

public record CustomerMeDto(Guid Id, string FullName, string Phone);
public record AddressDto(Guid Id, string Street, string Number, string? Complement, string Neighborhood, string City, string State, string ZipCode, bool IsDefault);
