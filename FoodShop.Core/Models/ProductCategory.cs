namespace FoodShop.Core.Models;

public class ProductCategory : EntityBase
{
    public Guid? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
