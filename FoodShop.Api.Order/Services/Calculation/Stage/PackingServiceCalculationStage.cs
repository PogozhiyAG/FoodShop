using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class PackingServiceCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "packing";

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        return EnumerateCalculations(orderCalculationContext);
    }

    private IEnumerable<OrderCalculation> EnumerateCalculations(OrderCalculationContext orderCalculationContext)
    {
        if(orderCalculationContext.Order.Items.Count > 0)
        {
            yield return orderCalculationContext.CreateCalculation(c =>
            {
                c.TypeCode = OrderCalculationTypeCodes.Service;
                //TODO: from DB
                c.Amount = .5M;
            });
        }
    }
}