namespace FoodShop.Core.Models;

public class ProductPriceStrategyLink : EntityBase
{
    public static ProductPriceStrategyLink Default { get; } = new() { ProductPriceStrategy = ProductPriceStrategy.Default };
    public int Priority { get; set; }
    public Guid? TokenTypeId { get; set; }
    public TokenType? TokenType { get; set; }
    public EntityTypeCode ReferenceType { get; set; }
    public Guid ReferenceId { get; set; }
    public Guid ProductPriceStrategyId { get; set; }
    public ProductPriceStrategy ProductPriceStrategy { get; set; }

}
