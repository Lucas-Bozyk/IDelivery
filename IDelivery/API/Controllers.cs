using System.Security.Claims;
using IDelivery.Application.DTOs.Auth;
using IDelivery.Application.DTOs.Coupons;
using IDelivery.Application.IServices;
using IDelivery.Application.UseCases;
using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Api;

[ApiController, Route("api/auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request) => Ok(await service.RegisterAsync(request));

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request) => Ok(await service.LoginAsync(request));

    [HttpPost("refresh-token"), AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request) => Ok(await service.RefreshAsync(request));
}

[ApiController, Route("api/customers"), Authorize(Roles = "Customer")]
public class CustomersController(DeliveryDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] Customer request)
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) return NotFound();
        customer.FullName = request.FullName;
        customer.Phone = request.Phone;
        customer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] CustomerAddress request)
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) return NotFound();
        request.CustomerId = customer.Id;
        db.CustomerAddresses.Add(request);
        await db.SaveChangesAsync();
        return Ok(request);
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) return Ok(Array.Empty<CustomerAddress>());
        return Ok(await db.CustomerAddresses.Where(x => x.CustomerId == customer.Id).ToListAsync());
    }

    [HttpPut("addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] CustomerAddress request)
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) return NotFound();
        var addr = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customer.Id);
        if (addr is null) return NotFound();
        addr.Street = request.Street;
        addr.Number = request.Number;
        addr.Complement = request.Complement;
        addr.Neighborhood = request.Neighborhood;
        addr.City = request.City;
        addr.State = request.State;
        addr.ZipCode = request.ZipCode;
        addr.IsDefault = request.IsDefault;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("addresses/{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        var userId = User.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) return NotFound();
        var addr = await db.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customer.Id);
        if (addr is null) return NotFound();
        db.CustomerAddresses.Remove(addr);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController, Route("api/restaurants")]
public class RestaurantsController(DeliveryDbContext db, CreateRestaurantUseCase createRestaurantUseCase) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok(await db.Restaurants.ToListAsync());
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id) => Ok(await db.Restaurants.FirstOrDefaultAsync(x => x.Id == id));

    [HttpPost, Authorize(Roles = "Admin,RestaurantOwner")]
    public async Task<IActionResult> Create([FromBody] CreateRestaurantCommand command) => Ok(await createRestaurantUseCase.ExecuteAsync(command));

    [HttpPut("{id:guid}"), Authorize(Roles = "Admin,RestaurantOwner")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Restaurant request)
    {
        var entity = await db.Restaurants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Cnpj = request.Cnpj;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.CategoryId = request.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:guid}/open-status"), Authorize(Roles = "Admin,RestaurantOwner")]
    public async Task<IActionResult> OpenStatus(Guid id, [FromBody] bool isOpen)
    {
        var entity = await db.Restaurants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        entity.IsOpen = isOpen;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{restaurantId:guid}/products")]
    public async Task<IActionResult> ListProducts(Guid restaurantId) => Ok(await db.Products.Where(x => x.RestaurantId == restaurantId).ToListAsync());

    [HttpPost("{restaurantId:guid}/products"), Authorize(Roles = "Admin,RestaurantOwner")]
    public async Task<IActionResult> CreateProduct(Guid restaurantId, [FromBody] Product request)
    {
        request.RestaurantId = restaurantId;
        db.Products.Add(request);
        await db.SaveChangesAsync();
        return Ok(request);
    }

    [HttpGet("{restaurantId:guid}/reviews")]
    public async Task<IActionResult> ListReviews(Guid restaurantId) => Ok(await db.Reviews.Where(x => x.RestaurantId == restaurantId).ToListAsync());
}

[ApiController, Route("api/products"), Authorize(Roles = "Admin,RestaurantOwner")]
public class ProductsController(DeliveryDbContext db) : ControllerBase
{
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product request)
    {
        var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.PromotionalPrice = request.PromotionalPrice;
        entity.IsAvailable = request.IsAvailable;
        entity.ImageUrl = request.ImageUrl;
        entity.MenuCategoryId = request.MenuCategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await db.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) return NotFound();
        db.Products.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController, Route("api/cart"), Authorize(Roles = "Customer")]
public class CartController(DeliveryDbContext db, AddProductToCartUseCase addProductToCartUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customerId = await User.GetCustomerIdAsync(db);
        var cart = await db.Carts.Include(x => x.Items).FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsActive);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddProductToCartCommand request)
    {
        var customerId = await User.GetCustomerIdAsync(db);
        var cart = await addProductToCartUseCase.ExecuteAsync(request with { CustomerId = customerId });
        return Ok(cart);
    }

    [HttpPut("items/{id:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] int quantity)
    {
        var customerId = await User.GetCustomerIdAsync(db);
        var item = await db.CartItems.Join(db.Carts, i => i.CartId, c => c.Id, (i, c) => new { i, c })
            .Where(x => x.i.Id == id && x.c.CustomerId == customerId && x.c.IsActive)
            .Select(x => x.i)
            .FirstOrDefaultAsync();
        if (item is null) return NotFound();
        item.Quantity = quantity;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id)
    {
        var customerId = await User.GetCustomerIdAsync(db);
        var item = await db.CartItems.Join(db.Carts, i => i.CartId, c => c.Id, (i, c) => new { i, c })
            .Where(x => x.i.Id == id && x.c.CustomerId == customerId && x.c.IsActive)
            .Select(x => x.i)
            .FirstOrDefaultAsync();
        if (item is null) return NotFound();
        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        var customerId = await User.GetCustomerIdAsync(db);
        var cart = await db.Carts.Include(x => x.Items).FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsActive);
        if (cart is null) return NoContent();
        db.CartItems.RemoveRange(cart.Items);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController, Route("api/orders"), Authorize(Roles = "Customer")]
public class OrdersController(DeliveryDbContext db, IOrderService service, CreateOrderUseCase createOrderUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Guid? couponId)
    {
        var customerId = await User.GetCustomerIdAsync(db);
        return Ok(await createOrderUseCase.ExecuteAsync(new CreateOrderCommand(customerId, couponId)));
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customerId = await User.GetCustomerIdAsync(db);
        return Ok(await service.GetByCustomerAsync(customerId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => Ok(await service.GetByIdAsync(id));

    [HttpPatch("{id:guid}/status"), Authorize(Roles = "Admin,RestaurantOwner")]
    public async Task<IActionResult> Status(Guid id, [FromBody] OrderStatus status) { await service.UpdateStatusAsync(id, status); return NoContent(); }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id) { await service.UpdateStatusAsync(id, OrderStatus.Cancelled); return NoContent(); }
}

[ApiController, Route("api/payments"), Authorize(Roles = "Customer")]
public class PaymentsController(ProcessPaymentUseCase processPaymentUseCase, DeliveryDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProcessPaymentCommand request) => Ok(await processPaymentUseCase.ExecuteAsync(request));

    [HttpPost("webhook"), AllowAnonymous]
    public async Task<IActionResult> Webhook([FromBody] Guid paymentId)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(x => x.Id == paymentId);
        if (payment is null) return NotFound();
        if (payment.Status == PaymentStatus.Approved)
        {
            var order = await db.Orders.FirstAsync(x => x.Id == payment.OrderId);
            order.Status = OrderStatus.Confirmed;
            await db.SaveChangesAsync();
        }
        return Ok();
    }
}

[ApiController, Route("api/deliveries"), Authorize(Roles = "Admin,DeliveryDriver")]
public class DeliveriesController(DeliveryDbContext db, AssignDeliveryDriverUseCase assignDeliveryDriverUseCase) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List() => Ok(await db.Deliveries.ToListAsync());

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> Status(Guid id, [FromBody] DeliveryStatus status)
    {
        var delivery = await db.Deliveries.FirstOrDefaultAsync(x => x.Id == id);
        if (delivery is null) return NotFound();
        var payment = await db.Payments.FirstOrDefaultAsync(x => x.OrderId == delivery.OrderId);
        var paymentApproved = payment is not null && payment.Status == PaymentStatus.Approved;
        delivery.UpdateStatus(status, paymentApproved);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:guid}/assign-driver")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] Guid driverId) => Ok(await assignDeliveryDriverUseCase.ExecuteAsync(new AssignDeliveryDriverCommand(id, driverId)));
}

[ApiController, Route("api/coupons")]
public class CouponsController(DeliveryDbContext db) : ControllerBase
{
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Coupon request)
    {
        db.Coupons.Add(request);
        await db.SaveChangesAsync();
        return Ok(request);
    }

    [HttpGet("{code}/validate"), AllowAnonymous]
    public async Task<IActionResult> Validate(string code, [FromQuery] decimal orderTotal = 0)
    {
        var coupon = await db.Coupons.FirstOrDefaultAsync(x => x.Code == code);
        if (coupon is null) return Ok(new CouponValidationDto(code, false));
        var valid = coupon.ExpirationDate >= DateTime.UtcNow && orderTotal >= coupon.MinValue && coupon.UsedCount < coupon.UsageLimit;
        return Ok(new CouponValidationDto(code, valid));
    }
}

[ApiController, Route("api/reviews"), Authorize(Roles = "Customer")]
public class ReviewsController(CreateReviewUseCase createReviewUseCase, DeliveryDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewCommand request)
    {
        var customerId = await User.GetCustomerIdAsync(db);
        return Ok(await createReviewUseCase.ExecuteAsync(request with { CustomerId = customerId }));
    }
}

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("sub");
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }

    public static async Task<Guid> GetCustomerIdAsync(this ClaimsPrincipal user, DeliveryDbContext db)
    {
        var userId = user.GetUserId();
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (customer is null) throw new InvalidOperationException("Customer profile not found.");
        return customer.Id;
    }
}

[ApiController, Route("api/dev"), Authorize(Roles = "Admin")]
public class DevController(IDelivery.Application.UseCases.ValidateRelationshipsFlowUseCase validator) : ControllerBase
{
    [HttpPost("validate-relationships-flow")]
    public async Task<IActionResult> ValidateRelationshipsFlow(CancellationToken ct) => Ok(await validator.ExecuteAsync(ct));
}
