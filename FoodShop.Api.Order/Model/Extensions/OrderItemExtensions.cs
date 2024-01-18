using FoodShop.Api.Order.Dto.Catalog;

namespace FoodShop.Api.Order.Model.Extensions;

public static class OrderItemExtensions
{
    public static ProductBatchInfoRequest ToProductBatchInfoRequest(this IEnumerable<OrderItem> orderItems)
    {
        return new ProductBatchInfoRequest()
        {
            Items = orderItems.ToDictionary(i => i.ProductId, i => i.Quantity)
        };
    }
}
