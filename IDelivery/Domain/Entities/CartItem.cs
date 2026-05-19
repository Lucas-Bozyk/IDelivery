namespace IDelivery.Domain;

public class CartItem : EntityBase
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
