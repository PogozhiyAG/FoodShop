namespace FoodShop.Core.Models;

public class Product : EntityBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal Popularity { get; set; }
    public decimal CustomerRating { get; set; }
    public int? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public Uri? ImageUri { get; set; }
    public ICollection<ProductTagRelation> Tags { get; set; }
}
