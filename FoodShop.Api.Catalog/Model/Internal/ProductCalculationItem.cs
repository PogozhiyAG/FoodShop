using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Model.Internal;

public class ProductCalculationItem
{
    public required Product Product { get; set; }
    public required ProductPriceStrategyLink PriceStrategyLink { get; set; }
    public int Quantity { get; set; }
    public decimal OfferPrice{ get; set; }
    public decimal Amount { get; set; }
    public decimal OfferAmount { get; set; }
}
