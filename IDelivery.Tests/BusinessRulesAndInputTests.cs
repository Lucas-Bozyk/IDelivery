using IDelivery.Domain;
using IDelivery.Domain.ValueObjects;

namespace IDelivery.Tests;

public class BusinessRulesAndInputTests
{
    [Fact]
    public void Cart_Should_Reject_Unavailable_Product()
    {
        var cart = new Cart { CustomerId = Guid.NewGuid() };
        var product = new Product { Id = Guid.NewGuid(), RestaurantId = Guid.NewGuid(), IsAvailable = false };
        Assert.Throws<InvalidOperationException>(() => cart.AddProduct(product, 1));
    }

    [Fact]
    public void Cart_Should_Reject_Product_From_Another_Restaurant()
    {
        var restA = Guid.NewGuid();
        var restB = Guid.NewGuid();
        var cart = new Cart { CustomerId = Guid.NewGuid(), RestaurantId = restA };
        var product = new Product { Id = Guid.NewGuid(), RestaurantId = restB, IsAvailable = true };
        Assert.Throws<InvalidOperationException>(() => cart.AddProduct(product, 1));
    }

    [Fact]
    public void Delivery_Should_Require_Approved_Payment_To_Complete()
    {
        var delivery = new Delivery();
        Assert.Throws<InvalidOperationException>(() => delivery.UpdateStatus(DeliveryStatus.Delivered, false));
    }

    [Fact]
    public void Review_Should_Require_Completed_Order()
    {
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid(), 10, 2, 0);
        Assert.Throws<InvalidOperationException>(() => Review.Create(order.CustomerId, order, 5, "x"));
    }

    [Fact]
    public void ValueObjects_Should_Validate_Input()
    {
        Assert.Throws<ArgumentException>(() => new Cpf("123"));
        Assert.Throws<ArgumentException>(() => new Cnpj("123"));
        Assert.Throws<ArgumentException>(() => new EmailAddress("invalido"));
        Assert.Throws<ArgumentException>(() => new PhoneNumber("11"));
        Assert.Throws<ArgumentException>(() => new AddressVo("", "1", null, "B", "C", "SP", "01001000").Validate());
    }
}
