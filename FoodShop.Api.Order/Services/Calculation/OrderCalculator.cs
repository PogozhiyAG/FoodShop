using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services.Calculation.Stage;

namespace FoodShop.Api.Order.Services.Calculation;

public interface IOrderCalculator
{
    Task<OrderCalculationContext> CalculateOrder(Model.Order order);
}


public class OrderCalculator : IOrderCalculator
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public OrderCalculator(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public async Task<OrderCalculationContext> CalculateOrder(Model.Order order)
    {
        order.OrderCalculations.Clear();

        var context = new OrderCalculationContext()
        {
            Order = order
        };

        var stageKeys = _configuration["OrderCalculator:Stages"].Split(",", StringSplitOptions.RemoveEmptyEntries);

        foreach (var stageKey in stageKeys)
        {
            var calculationStage = _serviceProvider.GetRequiredKeyedService<IOrderCalculationStage>(stageKey.Trim());
            var calculations = await calculationStage.GetCalculation(context);

            foreach (var calculation in calculations)
            {
                calculation.Amount = Math.Round(calculation.Amount, 2);
                if (calculation.Amount != 0.0M)
                {
                    order.OrderCalculations.Add(calculation);
                }
            }
        }

        return context;
    }
}
