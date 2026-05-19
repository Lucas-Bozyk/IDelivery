using IDelivery.Domain;
using IDelivery.Domain.Interfaces.IRepositories;
using IDelivery.Persistence.Repositories;

namespace IDelivery.Persistence;

public static class PersistenceSetup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, DeliveryUnitOfWork>();
        return services;
    }
}
