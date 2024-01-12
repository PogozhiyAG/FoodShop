using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class DeliveryCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "delivery";

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        if (orderCalculationContext.Order.DeliveryInfo == null)
        {
            return Array.Empty<OrderCalculation>();
        }

        return new OrderCalculation[] { orderCalculationContext.CreateCalculation(c => {
            c.TypeCode = OrderCalculationTypeCodes.Delivery;
            //TODO: from DB or delivery service
            c.Amount = 3.5M;
        })};
    }
}
