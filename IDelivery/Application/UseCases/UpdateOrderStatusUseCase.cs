using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status);

public class UpdateOrderStatusUseCase(DeliveryDbContext db)
{
    public async Task ExecuteAsync(UpdateOrderStatusCommand command, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == command.OrderId, ct)
            ?? throw new InvalidOperationException("Order not found.");
        order.UpdateStatus(command.Status);
        await db.SaveChangesAsync(ct);
    }
}
