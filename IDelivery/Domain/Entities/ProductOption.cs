namespace IDelivery.Domain;

public class ProductOption : EntityBase
{
    public Guid ProductOptionGroupId { get; set; }
    public string Name { get; set; } = "";
    public decimal AdditionalPrice { get; set; }
}
