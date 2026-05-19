using IDelivery.Domain;
using IDelivery.Infrastructure;
using IDelivery.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Application.UseCases;

public record ProcessPaymentCommand(Guid OrderId, PaymentMethod Method, decimal Amount);

public class ProcessPaymentUseCase(DeliveryDbContext db, IPaymentGateway paymentGateway)
{
    public async Task<Payment> ExecuteAsync(ProcessPaymentCommand command, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == command.OrderId, ct)
            ?? throw new InvalidOperationException("Order not found.");
        var approved = await paymentGateway.ProcessAsync(command.Amount, command.Method.ToString());
        var payment = Payment.Create(order.Id, command.Method, command.Amount, approved);

        db.Payments.Add(payment);
        if (approved) order.ConfirmPayment();

        await db.SaveChangesAsync(ct);
        return payment;
    }
}
