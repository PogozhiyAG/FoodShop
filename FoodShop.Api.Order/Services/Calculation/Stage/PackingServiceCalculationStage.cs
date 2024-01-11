using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class PackingServiceCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "packing";

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        return new OrderCalculation[] { new OrderCalculation() {
            TypeCode = OrderCalculationTypeCodes.Service,
            //TODO: from DB
            Amount = 0.5M,
            //TODO: to OrderCalculator level?
            Order = orderCalculationContext.Order,
            CreateDate = orderCalculationContext.Now,
        } };
    }
}