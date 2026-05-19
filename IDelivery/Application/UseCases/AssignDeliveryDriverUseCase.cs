using IDelivery.Domain;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record AssignDeliveryDriverCommand(Guid DeliveryId, Guid DeliveryDriverId);

public class AssignDeliveryDriverUseCase(DeliveryDbContext db)
{
    public async Task<Delivery> ExecuteAsync(AssignDeliveryDriverCommand command, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries.FirstOrDefaultAsync(x => x.Id == command.DeliveryId, ct)
            ?? throw new InvalidOperationException("Delivery not found.");
        var driverExists = await db.DeliveryDrivers.AnyAsync(x => x.Id == command.DeliveryDriverId, ct);
        if (!driverExists) throw new InvalidOperationException("Driver not found.");

        delivery.AssignDriver(command.DeliveryDriverId);
        await db.SaveChangesAsync(ct);
        return delivery;
    }
}
