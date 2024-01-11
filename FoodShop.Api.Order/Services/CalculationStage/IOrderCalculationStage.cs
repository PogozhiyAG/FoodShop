using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.CalculationStage;

public interface IOrderCalculationStage
{
    Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext);
}
