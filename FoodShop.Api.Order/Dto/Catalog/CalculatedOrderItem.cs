namespace FoodShop.Api.Order.Dto.Catalog;

public class CalculatedOrderItem
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string? TokenTypeCode { get; set; }
    public string? StrategyName { get; set; }
    public decimal OfferPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal OfferAmount { get; set; }
}
