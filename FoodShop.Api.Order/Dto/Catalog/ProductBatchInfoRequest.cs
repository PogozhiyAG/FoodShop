namespace FoodShop.Api.Order.Dto.Catalog;

public class ProductBatchInfoRequest
{
    public Dictionary<string, int> Items { get; set; } = new();
}
