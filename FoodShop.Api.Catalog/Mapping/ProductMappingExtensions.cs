using FoodShop.Api.Catalog.Dto;
using FoodShop.Core.Models;

namespace FoodShop.Api.Catalog.Mapping;


public static class ProductMappingExtensions
{
    private static void SetupProductDto(ProductDto productDto, Product product)
    {
        productDto.Id = product.Id;
        productDto.Name = product.Name;
        productDto.Description = product.Description;
        productDto.Brand = product.Brand != null
            ? new()
            {
                Id = product.Brand!.Id,
                Name = product.Brand!.Name,
            }
            : null;
        productDto.Category = product.Category != null
            ? new()
            {
                Id = product.Category!.Id,
                Name = product.Category!.Name,
            }
            : null;
        productDto.Popularity = product.Popularity;
        productDto.CustomerRating = product.CustomerRating;
        productDto.Price = product.Price;
    }

    private static void SetupOfferedProductDto(OfferedProductDto offeredProductDto, Product product, ProductPriceStrategyLink offerLink)
    {
        SetupProductDto(offeredProductDto, product);
        offeredProductDto.TokenTypeCode = offerLink.TokenTypeCode;
        offeredProductDto.StrategyName = offerLink.ProductPriceStrategy.Name;
        offeredProductDto.OfferPrice = offerLink.ProductPriceStrategy.GetAmount(product.Price, 1);
    }

    private static void SetupOfferedProductBatchDto(OfferedProductBatchDto offeredProductBatchDto, Product product, ProductPriceStrategyLink offerLink, int quantity)
    {
        SetupOfferedProductDto(offeredProductBatchDto, product, offerLink);
        offeredProductBatchDto.Quantity = quantity;
        offeredProductBatchDto.OfferAmount = offerLink.ProductPriceStrategy.GetAmount(product.Price, quantity);
    }

    public static ProductDto MapToProductDto(this Product product)
    {
        var result = new ProductDto();
        SetupProductDto(result, product);
        return result;
    }

    public static OfferedProductDto MapToOfferedProductDto(this Product product, ProductPriceStrategyLink offerLink)
    {
        var result = new OfferedProductDto();
        SetupOfferedProductDto(result, product, offerLink);
        return result;
    }

    public static OfferedProductBatchDto MapToOfferedProductBatchDto(this Product product, ProductPriceStrategyLink offerLink, int quantity)
    {
        var result = new OfferedProductBatchDto();
        SetupOfferedProductBatchDto(result, product, offerLink, quantity);
        return result;
    }
}