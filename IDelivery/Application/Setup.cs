namespace IDelivery.Application;

public static class ApplicationSetup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IDelivery.Application.Mappings.DomainToDtoProfile).Assembly);
        services.AddScoped<IDelivery.Application.UseCases.RegisterCustomerUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.CreateRestaurantUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.AddProductToCartUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.CreateOrderUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.ProcessPaymentUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.UpdateOrderStatusUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.AssignDeliveryDriverUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.CreateReviewUseCase>();
        services.AddScoped<IDelivery.Application.UseCases.ValidateRelationshipsFlowUseCase>();
        services.AddScoped<IDelivery.Application.IServices.IAuthService, IDelivery.Application.Services.AuthService>();
        services.AddScoped<IDelivery.Application.IServices.ICustomerService, IDelivery.Application.Services.CustomerService>();
        services.AddScoped<IDelivery.Application.IServices.IRestaurantService, IDelivery.Application.Services.RestaurantService>();
        services.AddScoped<IDelivery.Application.IServices.IProductService, IDelivery.Application.Services.ProductService>();
        services.AddScoped<IDelivery.Application.IServices.IOrderService, IDelivery.Application.Services.OrderService>();
        return services;
    }
}
