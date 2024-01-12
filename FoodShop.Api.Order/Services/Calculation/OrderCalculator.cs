using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Services.Calculation.Stage;

namespace FoodShop.Api.Order.Services.Calculation;

public interface IOrderCalculator
{
    Task<IEnumerable<OrderCalculation>> CalculateOrder(Model.Order order);
}


public class OrderCalculationContext
{
    public required Model.Order Order { get; set; }
    public IEnumerable<OrderCalculation> CalculationResult { get; set; }
    public DateTime Now { get; set; } = DateTime.UtcNow;

    public OrderCalculation CreateCalculation(Action<OrderCalculation> calculationSetup) {
        var result = new OrderCalculation() {
            Order = Order,
            CreateDate = Now
        };

        if(calculationSetup != null)
        {
            calculationSetup(result);
        }

        return result;
    }
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

    public async Task<IEnumerable<OrderCalculation>> CalculateOrder(Model.Order order)
    {
        var calculationList = new List<OrderCalculation>();
        var context = new OrderCalculationContext()
        {
            Order = order,
            CalculationResult = calculationList
        };

        var stageKeys = _configuration["OrderCalculator:Stages"].Split(",", StringSplitOptions.RemoveEmptyEntries);

        foreach (var stageKey in stageKeys)
        {
            var calculationStage = _serviceProvider.GetRequiredKeyedService<IOrderCalculationStage>(stageKey.Trim());
            var calculation = await calculationStage.GetCalculation(context);
            calculationList.AddRange(calculation);
        }

        return calculationList;
    }
}
