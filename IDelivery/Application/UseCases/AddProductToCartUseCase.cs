using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record AddProductToCartCommand(Guid CustomerId, Guid ProductId, int Quantity);

public class AddProductToCartUseCase(DeliveryDbContext db)
{
    public async Task<Cart> ExecuteAsync(AddProductToCartCommand command, CancellationToken ct = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == command.ProductId, ct)
            ?? throw new InvalidOperationException("Product not found.");

        var cart = await db.Carts.Include(x => x.Items).FirstOrDefaultAsync(x => x.CustomerId == command.CustomerId && x.IsActive, ct);
        if (cart is null)
        {
            cart = new Cart { CustomerId = command.CustomerId, IsActive = true };
            db.Carts.Add(cart);
        }

        cart.AddProduct(product, command.Quantity);
        await db.SaveChangesAsync(ct);
        return cart;
    }
}
