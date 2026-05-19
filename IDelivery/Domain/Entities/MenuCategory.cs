namespace IDelivery.Domain;

public class MenuCategory : EntityBase
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = "";
}
