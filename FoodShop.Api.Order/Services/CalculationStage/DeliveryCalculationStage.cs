using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.CalculationStage;

public class DeliveryCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "delivery";

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        if(orderCalculationContext.Order.DeliveryInfo == null) {
            return Array.Empty<OrderCalculation>();
        }

        return new OrderCalculation[] { new OrderCalculation() {
            TypeCode = OrderCalculationTypeCodes.Delivery,
            //TODO: from DB or delivery service
            Amount = 3.5M,
            //TODO: to OrderCalculator level?
            Order = orderCalculationContext.Order,
            CreateDate = orderCalculationContext.Now,
        } };
    }
}
