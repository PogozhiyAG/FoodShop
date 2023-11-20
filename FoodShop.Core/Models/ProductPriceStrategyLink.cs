namespace FoodShop.Core.Models;

public class ProductPriceStrategyLink : EntityBase
{
    public static ProductPriceStrategyLink Default { get; } = new() { ProductPriceStrategy = ProductPriceStrategy.Default };
    public int Priority { get; set; }
    public int? TokenTypeId { get; set; }
    public TokenType? TokenType { get; set; }
    public EntityTypeCode ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public int ProductPriceStrategyId { get; set; }
    public ProductPriceStrategy ProductPriceStrategy { get; set; }

}
