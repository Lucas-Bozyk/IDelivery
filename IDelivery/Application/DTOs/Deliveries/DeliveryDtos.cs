using IDelivery.Domain;

namespace IDelivery.Application.DTOs.Deliveries;

public record DeliveryDto(Guid Id, DeliveryStatus Status, Guid? DeliveryDriverId);
