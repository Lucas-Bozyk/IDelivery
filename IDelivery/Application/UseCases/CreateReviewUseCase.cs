using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record CreateReviewCommand(Guid CustomerId, Guid OrderId, int Rating, string Comment);

public class CreateReviewUseCase(DeliveryDbContext db)
{
    public async Task<Review> ExecuteAsync(CreateReviewCommand command, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == command.OrderId && x.CustomerId == command.CustomerId, ct)
            ?? throw new InvalidOperationException("Order not found.");
        var review = Review.Create(command.CustomerId, order, command.Rating, command.Comment);
        db.Reviews.Add(review);
        await db.SaveChangesAsync(ct);
        return review;
    }
}
