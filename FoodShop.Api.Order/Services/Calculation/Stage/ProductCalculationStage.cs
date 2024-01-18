using FoodShop.Api.Order.Dto.Catalog;
using FoodShop.Api.Order.Model;
using FoodShop.Api.Order.Model.Extensions;

namespace FoodShop.Api.Order.Services.Calculation.Stage;

public class ProductCalculationStage : IOrderCalculationStage
{
    public const string DEFAULT_SERVICE_KEY = "product";

    public const string PROPERTY_PRICE_STRATEGY = "S";
    public const string PROPERTY_TOKEN_TYPE = "T";

    private readonly IProductCatalog _productCatalog;

    public ProductCalculationStage(IProductCatalog productCatalog)
    {
        _productCatalog = productCatalog;
    }

    public async Task<IEnumerable<OrderCalculation>> GetCalculation(OrderCalculationContext orderCalculationContext)
    {
        orderCalculationContext.ProductBatchInfos = await _productCatalog.GetProductBatchInfos(orderCalculationContext.Order.Items.ToProductBatchInfoRequest());
        var itemsDictionary = orderCalculationContext.Order.Items.ToDictionary(i => i.ProductId);
        var result = orderCalculationContext.ProductBatchInfos.SelectMany(p => GetOrderItemCalculation(orderCalculationContext, p, itemsDictionary[p.Id.ToString()]));
        return result;
    }

    private IEnumerable<OrderCalculation> GetOrderItemCalculation(OrderCalculationContext orderCalculationContext, ProductBatchInfo item, OrderItem orderItem)
    {
        yield return orderCalculationContext.CreateCalculation(c => {
            c.OrderItemId = orderItem.Id;
            c.OrderItem = orderItem;
            c.TypeCode = OrderCalculationTypeCodes.Product;
            c.Amount = item.Amount;
        });

        var discount = item.OfferAmount - item.Amount;
        if(discount != 0)
        {
            yield return orderCalculationContext.CreateCalculation(c => {
                c.OrderItemId = orderItem.Id;
                c.OrderItem = orderItem;
                c.TypeCode = OrderCalculationTypeCodes.ProductDiscount;
                c.Amount = discount;

                if (!string.IsNullOrEmpty(item.StrategyName))
                {
                    c.Properties.Add(new() { Name = PROPERTY_PRICE_STRATEGY, Value = item.StrategyName });
                }

                if (!string.IsNullOrEmpty(item.TokenTypeCode))
                {
                    c.Properties.Add(new() { Name = PROPERTY_TOKEN_TYPE, Value = item.TokenTypeCode });
                }
            });
        }
    }
}