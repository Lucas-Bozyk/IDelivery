namespace IDelivery.Domain;

public class Cart : EntityBase
{
    public Guid CustomerId { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? RestaurantId { get; set; }
    public List<CartItem> Items { get; set; } = [];

    public void AddProduct(Product product, int quantity)
    {
        if (!product.IsAvailable) throw new InvalidOperationException("Produto indisponivel nao pode ser adicionado ao carrinho.");
        if (RestaurantId.HasValue && RestaurantId.Value != product.RestaurantId)
            throw new InvalidOperationException("Pedido/carrinho so pode conter produtos de um unico restaurante.");

        var existing = Items.FirstOrDefault(x => x.ProductId == product.Id);
        if (existing is null) Items.Add(new CartItem { ProductId = product.Id, Quantity = quantity });
        else existing.Quantity += quantity;
        RestaurantId = product.RestaurantId;
    }
}
