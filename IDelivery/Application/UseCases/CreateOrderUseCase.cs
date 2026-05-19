using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record CreateOrderCommand(Guid CustomerId, Guid? CouponId);

public class CreateOrderUseCase(DeliveryDbContext db)
{
    public async Task<Order> ExecuteAsync(CreateOrderCommand command, CancellationToken ct = default)
    {
        var hasAddress = await db.CustomerAddresses.AnyAsync(x => x.CustomerId == command.CustomerId, ct);
        if (!hasAddress) throw new InvalidOperationException("Cliente nao pode criar pedido sem endereco valido.");

        var cart = await db.Carts.Include(x => x.Items).FirstOrDefaultAsync(x => x.CustomerId == command.CustomerId && x.IsActive, ct)
            ?? throw new InvalidOperationException("Active cart not found.");
        if (cart.RestaurantId is null) throw new InvalidOperationException("Cart without restaurant.");

        var restaurant = await db.Restaurants.FirstOrDefaultAsync(x => x.Id == cart.RestaurantId.Value, ct)
            ?? throw new InvalidOperationException("Restaurant not found.");
        if (!restaurant.IsOpen) throw new InvalidOperationException("Restaurante fechado nao pode receber pedido.");

        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync(ct);
        if (products.Any(p => p.RestaurantId != restaurant.Id))
            throw new InvalidOperationException("Pedido so pode conter produtos de um unico restaurante.");

        var subtotal = cart.Items.Sum(i =>
        {
            var p = products.First(x => x.Id == i.ProductId);
            return (p.PromotionalPrice ?? p.Price) * i.Quantity;
        });
        var deliveryFee = subtotal >= 60m ? 0m : 8m;
        var discount = 0m;

        if (command.CouponId.HasValue)
        {
            var coupon = await db.Coupons.FirstOrDefaultAsync(x => x.Id == command.CouponId.Value, ct)
                ?? throw new InvalidOperationException("Coupon not found.");
            discount = coupon.Apply(subtotal);
        }

        var order = Order.Create(command.CustomerId, restaurant.Id, subtotal, deliveryFee, discount);

        db.Orders.Add(order);
        cart.IsActive = false;
        await db.SaveChangesAsync(ct);
        return order;
    }
}
