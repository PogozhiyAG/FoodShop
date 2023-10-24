namespace FoodShop.Core.Models;

public class Product : EntityBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public Uri? ImageUri { get; set; }
}
