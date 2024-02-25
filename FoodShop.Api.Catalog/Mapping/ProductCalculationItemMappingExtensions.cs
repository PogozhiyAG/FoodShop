using FoodShop.Api.Catalog.Model.Internal;
using FoodShop.Catalog.Grpc;

namespace FoodShop.Api.Catalog.Mapping;

public static class ProductCalculationItemMappingExtensions
{
    public static ProductCalculationResponseItem ToProductCalculationResponseItem(this ProductCalculationItem item)
    {
        ProductCalculationResponseItem result = new();

        var product = item.Product;
        var offerLink = item.PriceStrategyLink;

        result.Id = product.Id;
        result.Name = product.Name;
        result.Description = product.Description;
        result.ImageUri = product.ImageUri?.ToString() ?? string.Empty;
        result.Brand = product.Brand != null
            ? new()
            {
                Id = product.Brand!.Id,
                Name = product.Brand!.Name,
            }
            : null;
        result.Category = product.Category != null
            ? new()
            {
                Id = product.Category!.Id,
                Name = product.Category!.Name,
            }
            : null;
        result.Popularity = Convert.ToInt32(Math.Round(product.Popularity * 100));
        result.CustomerRating = Convert.ToInt32(Math.Round(product.CustomerRating * 100));
        result.Price = Convert.ToInt32(Math.Round(product.Price * 100));
        result.Tags.AddRange(product.Tags.Select(r => r.Tag.Name));
        result.TokenTypeCode = offerLink.TokenTypeCode ?? string.Empty;
        result.StrategyName = offerLink.ProductPriceStrategy.Name;
        result.OfferPrice = Convert.ToInt32(Math.Round(item.OfferPrice * 100));
        result.Quantity = item.Quantity;
        result.Amount = Convert.ToInt32(Math.Round(item.Amount * 100));
        result.OfferAmount = Convert.ToInt32(Math.Round(item.OfferAmount * 100));

        return result;
    }

}