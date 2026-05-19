namespace IDelivery.Domain;

public class Payment : EntityBase
{
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public string Gateway { get; set; } = "0";
    public string ExternalTransactionId { get; set; } = "";
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static Payment Create(Guid orderId, PaymentMethod method, decimal amount, bool approved)
    {
        return new Payment
        {
            OrderId = orderId,
            Method = method,
            Amount = amount,
            Status = approved ? PaymentStatus.Approved : PaymentStatus.Rejected,
            ExternalTransactionId = Guid.NewGuid().ToString("N"),
            Gateway = "0",
            PaidAt = approved ? DateTime.UtcNow : null
        };
    }
}
