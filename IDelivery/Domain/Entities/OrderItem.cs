namespace IDelivery.Domain;

public class OrderItem : EntityBase
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Observation { get; set; }
}
