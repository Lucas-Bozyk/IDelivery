namespace IDelivery.Domain;

public class Order : EntityBase
{
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public static Order Create(Guid customerId, Guid restaurantId, decimal subtotal, decimal deliveryFee, decimal discount)
    {
        return new Order
        {
            CustomerId = customerId,
            RestaurantId = restaurantId,
            Subtotal = subtotal,
            DeliveryFee = deliveryFee,
            Discount = discount,
            Total = subtotal + deliveryFee - discount,
            Status = OrderStatus.Created
        };
    }

    public void ConfirmPayment()
    {
        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Cancelled && newStatus == OrderStatus.OutForDelivery)
            throw new InvalidOperationException("Pedido cancelado nao pode ser entregue.");
        Status = newStatus;
        if (newStatus == OrderStatus.Cancelled) CancelledAt = DateTime.UtcNow;
        if (newStatus == OrderStatus.Completed) CompletedAt = DateTime.UtcNow;
    }

    public void EnsureCanBeReviewedBy(Guid customerId)
    {
        if (CustomerId != customerId) throw new InvalidOperationException("Order does not belong to customer.");
        if (Status != OrderStatus.Completed) throw new InvalidOperationException("Review so pode ser feito para pedido concluido.");
    }
}
