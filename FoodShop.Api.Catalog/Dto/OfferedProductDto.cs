using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Dto;

public class OfferedProductDto : ProductDto
{
    public ProductPriceStrategyLink OfferLink { get; set; }
    public decimal OfferPrice { get; set; }
}