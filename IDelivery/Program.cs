using System.Text;
using Asp.Versioning;
using IDelivery.Api;
using IDelivery.Application;
using IDelivery.Domain;
using IDelivery.Infrastructure.Config;
using IDelivery.Infrastructure;
using IDelivery.Infrastructure.Security;
using IDelivery.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

DotEnvLoader.Load(Path.Combine(AppContext.BaseDirectory, ".env"));
DotEnvLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IdentityDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("IdentityDb")));
builder.Services.AddDbContext<DeliveryDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("DeliveryDb")));
builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
var jwt = builder.Configuration.GetSection("Jwt");
if (string.IsNullOrWhiteSpace(jwt["Key"]) || jwt["Key"]!.Length < 32)
    throw new InvalidOperationException("JWT key must be configured via environment (Jwt__Key) with at least 32 chars.");
if (string.IsNullOrWhiteSpace(jwt["Issuer"]) || string.IsNullOrWhiteSpace(jwt["Audience"]))
    throw new InvalidOperationException("JWT issuer/audience must be configured via environment (Jwt__Issuer, Jwt__Audience).");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var deliveryDb = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
    identityDb.Database.EnsureCreated();
    deliveryDb.Database.EnsureCreated();
    foreach (var role in Enum.GetValues<UserRole>())
    {
        if (!identityDb.Roles.Any(x => x.Name == role)) identityDb.Roles.Add(new Role { Name = role });
    }
    SeedUserByRole(identityDb, deliveryDb, UserRole.Admin, "ADMIN_EMAIL", "ADMIN_PASSWORD");
    SeedUserByRole(identityDb, deliveryDb, UserRole.Customer, "CUSTOMER_EMAIL", "CUSTOMER_PASSWORD");
    SeedUserByRole(identityDb, deliveryDb, UserRole.RestaurantOwner, "RESTAURANT_OWNER_EMAIL", "RESTAURANT_OWNER_PASSWORD");
    SeedUserByRole(identityDb, deliveryDb, UserRole.DeliveryDriver, "DELIVERY_DRIVER_EMAIL", "DELIVERY_DRIVER_PASSWORD");
    identityDb.SaveChanges();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void SeedUserByRole(
    IdentityDbContext identityDb,
    DeliveryDbContext deliveryDb,
    UserRole role,
    string emailEnv,
    string passwordEnv)
{
    var email = Environment.GetEnvironmentVariable(emailEnv);
    var password = Environment.GetEnvironmentVariable(passwordEnv);
    if (string.IsNullOrWhiteSpace(email)) return;
    if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
        throw new InvalidOperationException($"{passwordEnv} must be set with at least 12 chars.");
    email = email.Trim().ToLowerInvariant();
    if (identityDb.Users.Any(x => x.Email == email)) return;

    var user = new User { Email = email, PasswordHash = PasswordHasher.HashPassword(password) };
    identityDb.Users.Add(user);
    identityDb.SaveChanges();

    var roleEntity = identityDb.Roles.First(x => x.Name == role);
    identityDb.UserRoles.Add(new UserRoleMap { UserId = user.Id, RoleId = roleEntity.Id });
    identityDb.SaveChanges();

    if (role == UserRole.Customer)
    {
        if (!deliveryDb.Customers.Any(x => x.UserId == user.Id))
        {
            deliveryDb.Customers.Add(new Customer
            {
                UserId = user.Id,
                FullName = email.Split('@')[0],
                Phone = "11999999999"
            });
            deliveryDb.SaveChanges();
        }
    }
    else if (role == UserRole.DeliveryDriver)
    {
        if (!deliveryDb.DeliveryDrivers.Any(x => x.Name == email))
        {
            var driver = new DeliveryDriver { Name = email };
            deliveryDb.DeliveryDrivers.Add(driver);
            deliveryDb.SaveChanges();
            user.DeliveryDriverId = driver.Id;
            identityDb.Users.Update(user);
            identityDb.SaveChanges();
        }
    }
}

public partial class Program { }
