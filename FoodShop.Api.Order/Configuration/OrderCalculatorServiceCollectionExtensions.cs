﻿using FoodShop.Api.Order.Services;
using FoodShop.Api.Order.Services.Calculation;
using FoodShop.Api.Order.Services.Calculation.Stage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FoodShop.Api.Order.Configuration;

public static class OrderCalculatorServiceCollectionExtensions
{
    public static IServiceCollection AddOrderCalculation(this IServiceCollection services)
    {
        services.TryAddScoped<IOrderCalculator, OrderCalculator>();

        services.TryAddScoped<IProductCatalog, ProductCatalogGrpc>();
        services.TryAddScoped<ICustomerProfile, CustomerProfile>();

        services.TryAddSingleton<IOrderAmountCorrectionsProvider, OrderAmountCorrectionsProvider>();

        //the order matters
        services.AddScoped<IOrderCalculationStage, ProductCalculationStage>();
        services.AddScoped<IOrderCalculationStage, PackingServiceCalculationStage>();
        services.AddScoped<IOrderCalculationStage, DeliveryCalculationStage>();
        services.AddScoped<IOrderCalculationStage, CorrectionCalculationStage>();

        return services;
    }
}