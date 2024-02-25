using FoodShop.Api.Catalog.Dto;
using FoodShop.Api.Catalog.Model.Internal;
using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Mapping;


public static class ProductMappingExtensions
{
    private static void SetupProductDto(ProductDto productDto, Product product)
    {
        productDto.Id = product.Id;
        productDto.Name = product.Name;
        productDto.Description = product.Description;
        productDto.ImageUri = product.ImageUri;
        productDto.Brand = product.Brand != null
            ? new() {
                    Id = product.Brand!.Id,
                    Name = product.Brand!.Name,
              }
            : null;
        productDto.Category = product.Category != null
            ? new() {
                    Id = product.Category!.Id,
                    Name = product.Category!.Name,
              }
            : null;
        productDto.Popularity = product.Popularity;
        productDto.CustomerRating = product.CustomerRating;
        productDto.Price = product.Price;
        productDto.Tags.AddRange(product.Tags.Select(r => r.Tag.Name));
    }

    private static void SetupOfferedProductDto(OfferedProductDto offeredProductDto, ProductCalculationItem item)
    {
        SetupProductDto(offeredProductDto, item.Product);
        offeredProductDto.TokenTypeCode = item.PriceStrategyLink.TokenTypeCode;
        offeredProductDto.StrategyName = item.PriceStrategyLink.ProductPriceStrategy.Name;
        offeredProductDto.OfferPrice = item.OfferPrice;
    }

    private static void SetupOfferedProductBatchDto(OfferedProductBatchDto offeredProductBatchDto, ProductCalculationItem item)
    {
        SetupOfferedProductDto(offeredProductBatchDto, item);
        offeredProductBatchDto.Quantity = item.Quantity;
        offeredProductBatchDto.Amount = item.Amount;
        offeredProductBatchDto.OfferAmount = item.OfferAmount;
    }

    public static ProductDto MapToProductDto(this ProductCalculationItem item)
    {
        var result = new ProductDto();
        SetupProductDto(result, item.Product);
        return result;
    }

    public static OfferedProductDto MapToOfferedProductDto(this ProductCalculationItem item)
    {
        var result = new OfferedProductDto();
        SetupOfferedProductDto(result, item);
        return result;
    }

    public static OfferedProductBatchDto MapToOfferedProductBatchDto(this ProductCalculationItem item)
    {
        var result = new OfferedProductBatchDto();
        SetupOfferedProductBatchDto(result, item);
        return result;
    }
}