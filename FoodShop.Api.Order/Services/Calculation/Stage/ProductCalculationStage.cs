using FoodShop.Api.Order.Dto.Catalog;
using FoodShop.Api.Order.Model;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class ProductCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "product";

    public const string PROPERTY_PRODUCT = "P";
    public const string PROPERTY_STRATEGY = "S";
    public const string PROPERTY_TOKEN_TYPE = "T";

    private readonly IProductCatalog _productCatalog;

    public ProductCalculationStage(IProductCatalog productCatalog)
    {
        _productCatalog = productCatalog;
    }

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        var calculatedProducts = await _productCatalog.CalculateProducts(orderCalculationContext.Order.Items);
        var result = calculatedProducts.SelectMany(p => GetOrderItemCalculation(orderCalculationContext, p));
        return result;
    }

    private IEnumerable<OrderCalculation> GetOrderItemCalculation(OrderCalculationContext orderCalculationContext, CalculatedOrderItem item)
    {
        yield return orderCalculationContext.CreateCalculation(c => {
            c.TypeCode = OrderCalculationTypeCodes.Product;
            c.Amount = item.Amount;
            c.Properties.Add(new() { Name = PROPERTY_PRODUCT, Value = item.Id.ToString() });
        });

        var discount = item.OfferAmount - item.Amount;
        if(discount != 0)
        {
            yield return orderCalculationContext.CreateCalculation(c => {
                c.TypeCode = OrderCalculationTypeCodes.ProductDiscount;
                c.Amount = discount;

                c.Properties.Add(new() { Name = PROPERTY_PRODUCT, Value = item.Id.ToString() });

                if (!string.IsNullOrEmpty(item.StrategyName))
                {
                    c.Properties.Add(new() { Name = PROPERTY_STRATEGY, Value = item.StrategyName });
                }

                if (!string.IsNullOrEmpty(item.TokenTypeCode))
                {
                    c.Properties.Add(new() { Name = PROPERTY_TOKEN_TYPE, Value = item.TokenTypeCode });
                }
            });
        }
    }
}