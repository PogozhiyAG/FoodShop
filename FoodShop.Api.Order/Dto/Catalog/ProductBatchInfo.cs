namespace FoodShop.Api.Order.Dto.Catalog;

public class ProductBatchInfo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public NamedDto? Brand { get; set; }
    public NamedDto? Category { get; set; }
    public string? ImageUri { get; set; }
    public decimal Popularity { get; set; }
    public decimal CustomerRating { get; set; }
    public decimal Price { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? TokenTypeCode { get; set; }
    public string? StrategyName { get; set; }
    public decimal OfferPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal OfferAmount { get; set; }
}
