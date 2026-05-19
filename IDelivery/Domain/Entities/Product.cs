namespace IDelivery.Domain;

public class Product : EntityBase
{
    public Guid RestaurantId { get; set; }
    public Guid MenuCategoryId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? PromotionalPrice { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<ProductOptionGroup> OptionGroups { get; set; } = [];
}
