namespace FoodShop.Api.Catalog.Dto;

public class ProductDto : NamedDto
{
    public string? Description { get; set; }
    public NamedDto? Brand { get; set; }
    public NamedDto? Category { get; set; }
    public decimal Popularity { get; set; }
    public decimal CustomerRating { get; set; }
    public decimal Price { get; set; }
}
