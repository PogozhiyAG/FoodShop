namespace FoodShop.Api.Catalog.Model;

public class ProductRequest
{
    public ProductSortType? Sort { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }

    public string? Text { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? TagId { get; set; }
}
