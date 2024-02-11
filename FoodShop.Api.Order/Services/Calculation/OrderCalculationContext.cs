using FoodShop.Api.Order.Dto.Catalog;
using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation;

public class OrderCalculationContext
{
    public required Model.Order Order { get; set; }
    public List<ProductBatchInfo> ProductBatchInfos { get; set; } = new();

    public OrderCalculation CreateCalculation(Action<OrderCalculation> calculationSetup)
    {
        var result = new OrderCalculation()
        {
            Id = Guid.NewGuid(),
            OrderId = Order.Id,
            Order = Order,
            CreateDate = DateTime.UtcNow
        };

        if (calculationSetup != null)
        {
            calculationSetup(result);
        }

        return result;
    }

    public decimal SumOf(params string[] typeCodes) =>
        Order.OrderCalculations
            .Where(r => typeCodes.Contains(r.TypeCode))
            .Sum(i => i.Amount);

}