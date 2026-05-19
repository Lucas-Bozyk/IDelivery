using IDelivery.Domain;

namespace IDelivery.Application.DTOs.Payments;

public record PaymentDto(Guid Id, PaymentStatus Status, decimal Amount, string ExternalTransactionId);
