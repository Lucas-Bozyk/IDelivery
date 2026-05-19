using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public class ValidateRelationshipsFlowUseCase(
    DeliveryDbContext db,
    AddProductToCartUseCase addToCart,
    CreateOrderUseCase createOrder,
    ProcessPaymentUseCase processPayment,
    AssignDeliveryDriverUseCase assignDriver,
    CreateReviewUseCase createReview)
{
    public async Task<object> ExecuteAsync(CancellationToken ct = default)
    {
        var suffix = DateTime.UtcNow.Ticks.ToString();

        var category = new RestaurantCategory { Name = $"Categoria {suffix}" };
        db.RestaurantCategories.Add(category);

        var restaurant = new Restaurant
        {
            Name = $"Rest {suffix}",
            Description = "validacao",
            Cnpj = "12345678000199",
            Phone = "11999999999",
            Email = $"rest{suffix}@local.test",
            CategoryId = category.Id,
            IsOpen = true
        };
        db.Restaurants.Add(restaurant);

        var menu = new MenuCategory { RestaurantId = restaurant.Id, Name = "Lanches" };
        db.MenuCategories.Add(menu);

        var product = new Product
        {
            RestaurantId = restaurant.Id,
            MenuCategoryId = menu.Id,
            Name = "Burger",
            Description = "ok",
            Price = 20,
            IsAvailable = true
        };
        db.Products.Add(product);

        var customer = new Customer { UserId = Guid.NewGuid(), FullName = "Flow User", Phone = "11999999999" };
        db.Customers.Add(customer);
        db.CustomerAddresses.Add(new CustomerAddress
        {
            CustomerId = customer.Id, Street = "Rua A", Number = "10", Neighborhood = "Centro", City = "SP", State = "SP", ZipCode = "01001000"
        });
        db.DeliveryDrivers.Add(new DeliveryDriver { Name = "Moto Teste" });
        await db.SaveChangesAsync(ct);

        var driver = await db.DeliveryDrivers.OrderByDescending(x => x.Id).FirstOrDefaultAsync(ct);
        await addToCart.ExecuteAsync(new AddProductToCartCommand(customer.Id, product.Id, 2), ct);
        var order = await createOrder.ExecuteAsync(new CreateOrderCommand(customer.Id, null), ct);
        var payment = await processPayment.ExecuteAsync(new ProcessPaymentCommand(order.Id, PaymentMethod.Pix, order.Total), ct);

        var delivery = new Delivery
        {
            OrderId = order.Id,
            AddressSnapshot = "Rua A, 10",
            EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(30)
        };
        db.Deliveries.Add(delivery);
        await db.SaveChangesAsync(ct);

        await assignDriver.ExecuteAsync(new AssignDeliveryDriverCommand(delivery.Id, driver!.Id), ct);
        delivery = await db.Deliveries.FirstAsync(x => x.Id == delivery.Id, ct);
        delivery.UpdateStatus(DeliveryStatus.Delivered, payment.Status == PaymentStatus.Approved);
        order = await db.Orders.FirstAsync(x => x.Id == order.Id, ct);
        order.UpdateStatus(OrderStatus.Completed);
        await db.SaveChangesAsync(ct);

        var review = await createReview.ExecuteAsync(new CreateReviewCommand(customer.Id, order.Id, 5, "fluxo ok"), ct);

        return new
        {
            restaurantId = restaurant.Id,
            productId = product.Id,
            orderId = order.Id,
            paymentId = payment.Id,
            deliveryId = delivery.Id,
            reviewId = review.Id
        };
    }
}
