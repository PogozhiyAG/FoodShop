namespace FoodShop.Core.Models;

public class Product : EntityBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public Uri? ImageUri { get; set; }
    public ICollection<ProductTagRelation> Tags { get; set; }
}
