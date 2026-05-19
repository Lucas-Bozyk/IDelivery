namespace IDelivery.Domain;

public class Delivery : EntityBase
{
    public Guid OrderId { get; set; }
    public Guid? DeliveryDriverId { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public string AddressSnapshot { get; set; } = "";
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public void AssignDriver(Guid driverId)
    {
        DeliveryDriverId = driverId;
        Status = DeliveryStatus.Assigned;
    }

    public void UpdateStatus(DeliveryStatus status, bool paymentApproved)
    {
        if (status == DeliveryStatus.Delivered && !paymentApproved)
            throw new InvalidOperationException("Entrega so pode ser concluida se pagamento estiver aprovado.");
        Status = status;
        if (status == DeliveryStatus.Delivered) DeliveredAt = DateTime.UtcNow;
    }
}
