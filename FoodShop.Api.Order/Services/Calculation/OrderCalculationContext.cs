using FoodShop.Api.Order.Model;
using System;

namespace FoodShop.Api.Order.Services.Calculation;

public class OrderCalculationContext
{
    public required Model.Order Order { get; set; }
    public IEnumerable<OrderCalculation> CalculationResult { get; set; }
    public DateTime Now { get; set; } = DateTime.UtcNow;

    public OrderCalculation CreateCalculation(Action<OrderCalculation> calculationSetup)
    {
        var result = new OrderCalculation()
        {
            Order = Order,
            CreateDate = Now
        };

        if (calculationSetup != null)
        {
            calculationSetup(result);
        }

        return result;
    }

    public decimal SumOf(params string[] typeCodes) =>
        CalculationResult
            .Where(r => typeCodes.Contains(r.TypeCode))
            .Select(i => i.Amount)
            .Sum();

}