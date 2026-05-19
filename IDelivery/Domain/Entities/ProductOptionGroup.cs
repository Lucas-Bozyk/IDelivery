namespace IDelivery.Domain;

public class ProductOptionGroup : EntityBase
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = "";
    public int MinSelection { get; set; }
    public int MaxSelection { get; set; }
    public List<ProductOption> Options { get; set; } = [];
}
