using System.Net;
using System.Net.Http.Json;
using IDelivery.Domain;
using IDelivery.Persistence;
using IDelivery.Tests.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Tests;

public class EndpointCoverageTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;
    public EndpointCoverageTests(TestApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Auth_Endpoints_Should_Work()
    {
        var client = _factory.CreateClient();
        var auth = await AuthClientHelper.RegisterAndLoginAsync(client, $"customer.{Guid.NewGuid():N}@test.local", "Customer");
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        Assert.False(string.IsNullOrWhiteSpace(auth.RefreshToken));

        var refresh = await client.PostAsJsonAsync("/api/auth/refresh-token", new IDelivery.Application.DTOs.Auth.RefreshTokenRequestDto(auth.RefreshToken));
        refresh.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Customer_Routes_Should_Work_With_Customer_Role()
    {
        var client = _factory.CreateClient();
        var auth = await AuthClientHelper.RegisterAndLoginAsync(client, $"customer2.{Guid.NewGuid():N}@test.local", "Customer");
        AuthClientHelper.SetBearer(client, auth.Token);

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/customers/me")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/customers/addresses", new CustomerAddress { Street = "Rua", Number = "1", Neighborhood = "Centro", City = "SP", State = "SP", ZipCode = "01001000" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/customers/addresses")).StatusCode);
    }

    [Fact]
    public async Task Restaurant_Product_Order_Payment_Delivery_Coupon_Review_Endpoints_Should_Work()
    {
        var client = _factory.CreateClient();
        var admin = await AuthClientHelper.RegisterAndLoginAsync(client, $"admin.{Guid.NewGuid():N}@test.local", "Admin");
        AuthClientHelper.SetBearer(client, admin.Token);

        Guid restaurantId;
        Guid productId;
        Guid driverId;
        Guid menuId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var category = new RestaurantCategory { Name = "Cat" };
            db.RestaurantCategories.Add(category);
            var driver = new DeliveryDriver { Name = "Driver" };
            db.DeliveryDrivers.Add(driver);
            await db.SaveChangesAsync();

            var createRestaurant = await client.PostAsJsonAsync("/api/restaurants", new IDelivery.Application.UseCases.CreateRestaurantCommand("R1", "Desc", "12345678000199", "11999999999", "r@test.local", category.Id));
            createRestaurant.EnsureSuccessStatusCode();
            var createdRestaurant = await createRestaurant.Content.ReadFromJsonAsync<Restaurant>();
            restaurantId = createdRestaurant!.Id;

            var menu = new MenuCategory { Name = "Menu", RestaurantId = restaurantId };
            db.MenuCategories.Add(menu);
            await db.SaveChangesAsync();
            menuId = menu.Id;

            var createProduct = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/products", new Product
            {
                Name = "P1", Description = "D", Price = 20, MenuCategoryId = menu.Id, IsAvailable = true
            });
            createProduct.EnsureSuccessStatusCode();
            var createdProduct = await createProduct.Content.ReadFromJsonAsync<Product>();
            productId = createdProduct!.Id;
            driverId = driver.Id;
        }

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/restaurants")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/restaurants/{restaurantId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsJsonAsync($"/api/restaurants/{restaurantId}/open-status", true)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/restaurants/{restaurantId}/products")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/products/{productId}", new Product { Name = "P2", Description = "D2", Price = 22, MenuCategoryId = menuId })).StatusCode);

        var customerClient = _factory.CreateClient();
        var customer = await AuthClientHelper.RegisterAndLoginAsync(customerClient, $"cflow.{Guid.NewGuid():N}@test.local", "Customer");
        AuthClientHelper.SetBearer(customerClient, customer.Token);
        await customerClient.PostAsJsonAsync("/api/customers/addresses", new CustomerAddress { Street = "Rua", Number = "2", Neighborhood = "Centro", City = "SP", State = "SP", ZipCode = "01001000" });

        Assert.Equal(HttpStatusCode.OK, (await customerClient.PostAsJsonAsync("/api/cart/items", new IDelivery.Application.UseCases.AddProductToCartCommand(Guid.Empty, productId, 2))).StatusCode);
        var createOrder = await customerClient.PostAsJsonAsync("/api/orders", (Guid?)null);
        createOrder.EnsureSuccessStatusCode();
        var order = await createOrder.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(order);
        Assert.Equal(HttpStatusCode.OK, (await customerClient.GetAsync("/api/orders")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await customerClient.GetAsync($"/api/orders/{order!.Id}")).StatusCode);

        var pay = await customerClient.PostAsJsonAsync("/api/payments", new IDelivery.Application.UseCases.ProcessPaymentCommand(order.Id, PaymentMethod.Pix, order.Total));
        pay.EnsureSuccessStatusCode();
        var payment = await pay.Content.ReadFromJsonAsync<Payment>();
        Assert.NotNull(payment);
        Assert.Equal(HttpStatusCode.OK, (await customerClient.PostAsJsonAsync("/api/payments/webhook", payment!.Id)).StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            db.Deliveries.Add(new Delivery { OrderId = order.Id, AddressSnapshot = "Rua 2", EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(30) });
            db.Coupons.Add(new Coupon { Code = "DESC10", DiscountType = DiscountType.Percentage, Value = 10, MinValue = 1, ExpirationDate = DateTime.UtcNow.AddDays(1), UsageLimit = 10 });
            await db.SaveChangesAsync();
        }

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/deliveries")).StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var delivery = await db.Deliveries.FirstAsync(x => x.OrderId == order.Id);
            Assert.Equal(HttpStatusCode.OK, (await client.PatchAsJsonAsync($"/api/deliveries/{delivery.Id}/assign-driver", driverId)).StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsJsonAsync($"/api/deliveries/{delivery.Id}/status", DeliveryStatus.Delivered)).StatusCode);
        }

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/coupons", new Coupon { Code = "C2", DiscountType = DiscountType.Fixed, Value = 5, MinValue = 1, ExpirationDate = DateTime.UtcNow.AddDays(1), UsageLimit = 10 })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/coupons/DESC10/validate?orderTotal=100")).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await customerClient.PostAsJsonAsync($"/api/orders/{order.Id}/cancel", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PatchAsJsonAsync($"/api/orders/{order.Id}/status", OrderStatus.Completed)).StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
            var persistedOrder = await db.Orders.FirstAsync(x => x.Id == order.Id);
            persistedOrder.UpdateStatus(OrderStatus.Completed);
            await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.OK, (await customerClient.PostAsJsonAsync("/api/reviews", new IDelivery.Application.UseCases.CreateReviewCommand(Guid.Empty, order.Id, 5, "ok"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/restaurants/{restaurantId}/reviews")).StatusCode);
    }
}
