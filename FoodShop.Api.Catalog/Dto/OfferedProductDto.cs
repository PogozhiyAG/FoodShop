using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Dto;

public class OfferedProductDto : ProductDto
{
    public string? TokenTypeCode { get; set; }
    public string? StrategyName { get; set; }
    public decimal OfferPrice { get; set; }
}