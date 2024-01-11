using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.CalculationStage;

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
            TypeCode = OrderCalculationTypeCodes.Product,
            Amount = p.OfferAmount,
            //TODO: to OrderCalculator level?
            Order = order,
            CreateDate = orderCalculationContext.Now,
        });

        return result;
    }
}