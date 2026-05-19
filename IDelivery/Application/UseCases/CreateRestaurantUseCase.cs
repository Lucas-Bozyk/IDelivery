using IDelivery.Domain;
using IDelivery.Persistence;

namespace IDelivery.Application.UseCases;

public record CreateRestaurantCommand(string Name, string Description, string Cnpj, string Phone, string Email, Guid CategoryId);

public class CreateRestaurantUseCase(DeliveryDbContext db)
{
    public async Task<Restaurant> ExecuteAsync(CreateRestaurantCommand command, CancellationToken ct = default)
    {
        var restaurant = new Restaurant
        {
            Name = command.Name,
            Description = command.Description,
            Cnpj = command.Cnpj,
            Phone = command.Phone,
            Email = command.Email,
            CategoryId = command.CategoryId,
            IsOpen = true
        };
        db.Restaurants.Add(restaurant);
        await db.SaveChangesAsync(ct);
        return restaurant;
    }
}
