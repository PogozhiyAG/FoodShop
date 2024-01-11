using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public interface IOrderCalculationStage
{
    Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext);
}
