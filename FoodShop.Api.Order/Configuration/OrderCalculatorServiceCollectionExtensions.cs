using FoodShop.Api.Order.Services;
using FoodShop.Api.Order.Services.Calculation;
using FoodShop.Api.Order.Services.Calculation.Stage;

namespace FoodShop.Api.Order.Configuration;

public static class OrderCalculatorServiceCollectionExtensions
{
    public static IServiceCollection AddOrderCalculation(this IServiceCollection services)
    {
        services.AddScoped<IOrderCalculator, OrderCalculator>();

        services.AddScoped<IProductCatalog, ProductCatalogGrpc>();
        services.AddScoped<ICustomerProfile, CustomerProfile>();

        services.AddSingleton<IOrderAmountCorrectionsProvider, OrderAmountCorrectionsProvider>();

        //the order matters
        services.AddScoped<IOrderCalculationStage, ProductCalculationStage>();
        services.AddScoped<IOrderCalculationStage, PackingServiceCalculationStage>();
        services.AddScoped<IOrderCalculationStage, DeliveryCalculationStage>();
        services.AddScoped<IOrderCalculationStage, CorrectionCalculationStage>();

        return services;
    }
}
