using FoodShop.Api.Order.Services.Calculation.Stage;

namespace FoodShop.Api.Order.Services.Calculation;

public interface IOrderCalculator
{
    Task<OrderCalculationContext> CalculateOrder(Model.Order order);
}


public class OrderCalculator(IServiceProvider _serviceProvider) : IOrderCalculator
{
    public async Task<OrderCalculationContext> CalculateOrder(Model.Order order)
    {
        order.OrderCalculations.Clear();

        var context = new OrderCalculationContext()
        {
            Order = order
        };

        foreach (var stage in _serviceProvider.GetServices<IOrderCalculationStage>())
        {
            var calculations = await stage.GetCalculation(context);

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
