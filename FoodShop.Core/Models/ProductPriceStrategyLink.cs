namespace FoodShop.Core.Models;

public class ProductPriceStrategyLink : EntityBase
{
    public static ProductPriceStrategyLink Default { get; } = new() { ProductPriceStrategy = ProductPriceStrategy.Default, Priority = 0 };
    public decimal Priority { get; set; }
    public string? TokenTypeCode { get; set; }
    public EntityTypeCode ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public int ProductPriceStrategyId { get; set; }
    public ProductPriceStrategy ProductPriceStrategy { get; set; }

}
