namespace IDelivery.Infrastructure;

public interface IEmailService { Task SendAsync(string to, string subject, string body); }
public interface IGeoLocationService { Task<string> ResolveAddressAsync(double lat, double lng); }
public interface IPaymentGateway { Task<bool> ProcessAsync(decimal amount, string method); }

public sealed class FakeEmailService : IEmailService { public Task SendAsync(string to, string subject, string body) => Task.CompletedTask; }
public sealed class FakeGeoLocationService : IGeoLocationService { public Task<string> ResolveAddressAsync(double lat, double lng) => Task.FromResult("Fake Address"); }
public sealed class FakePaymentGateway : IPaymentGateway { public Task<bool> ProcessAsync(decimal amount, string method) => Task.FromResult(true); }

public static class InfrastructureSetup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, FakeEmailService>();
        services.AddScoped<IGeoLocationService, FakeGeoLocationService>();
        services.AddScoped<IPaymentGateway, FakePaymentGateway>();
        return services;
    }
}
