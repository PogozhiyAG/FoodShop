using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services;

public interface IOrderCalculator
{
    Task<IEnumerable<OrderCalculation>> CalculateOrder(Model.Order order);
}


public class OrderCalculationContext
{
    public required Model.Order Order { get; set; }
    public IEnumerable<OrderCalculation> CalculationResult { get; set; }
    public DateTime Now { get; set; } = DateTime.UtcNow;
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

        foreach ( var stageKey in stageKeys )
        {
            var calculationStage = _serviceProvider.GetRequiredKeyedService<IOrderCalculationStage>(stageKey);
            var calculation = await calculationStage.GetCalculation(context);
            calculationList.AddRange(calculation);
        }

        return calculationList;
    }
}


public interface IOrderCalculationStage
{
    Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext);
}


public class ProductCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "product";
    private readonly IProductCatalog _productCatalog;

    public ProductCalculationStage(IProductCatalog productCatalog)
    {
        _productCatalog = productCatalog;
    }

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        var order = orderCalculationContext.Order;
        var calculatedProducts = await _productCatalog.CalculateProducts(order.Items);

        var result = calculatedProducts.Select(p => new OrderCalculation()
        {
            Order = order,
            TypeCode = OrderCalculationTypeCodes.Product,
            CreateDate = orderCalculationContext.Now,
            Amount = p.OfferAmount
        });

        return result;
    }
}