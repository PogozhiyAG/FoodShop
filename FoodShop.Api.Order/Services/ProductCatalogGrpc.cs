using FoodShop.Api.Order.Dto.Catalog;
using FoodShop.Catalog.Grpc;

namespace FoodShop.Api.Order.Services;

public class ProductCatalogGrpc : IProductCatalog
{
    private readonly ProductCalculator.ProductCalculatorClient _productCalculatorClient;

    public ProductCatalogGrpc(ProductCalculator.ProductCalculatorClient productCalculatorClient)
    {
        _productCalculatorClient = productCalculatorClient;
    }

    public async Task<List<ProductBatchInfo>> GetProductBatchInfos(ProductBatchInfoRequest request)
    {
        var apiRequest = new ProductCalculationRequest();
        apiRequest.Items.AddRange(
            request.Items.Select(kv => new ProductCalculationRequestItem()
            {
                ProductId = Convert.ToInt32(kv.Key),
                Quantity = kv.Value
            })
        );

        var response = await _productCalculatorClient.CalculateAsync(apiRequest);

        var result = response.Items.Select(i => i.ToProductBatchInfo()).ToList();
        return result;
    }
}


public static class ProductCalculationResponseItemExtensions
{
    public static ProductBatchInfo ToProductBatchInfo(this ProductCalculationResponseItem item)
    {
        var result = new ProductBatchInfo();

        result.Id = item.Id;
        result.Name = item.Name;
        result.Description = item.Description;
        result.Brand = new NamedDto()
        {
            Id = item.Brand.Id,
            Name = item.Brand.Name
        };
        result.Category = new NamedDto()
        {
            Id = item.Category.Id,
            Name = item.Category.Name
        };
        result.ImageUri = item.ImageUri;
        result.Popularity = item.Popularity * 0.01M;
        result.CustomerRating = item.CustomerRating * 0.01M;
        result.Price = item.Price * 0.01M;
        result.Tags.AddRange(item.Tags);
        result.TokenTypeCode = item.TokenTypeCode;
        result.StrategyName = item.StrategyName;
        result.OfferPrice = item.OfferPrice * 0.01M;
        result.Quantity = item.Quantity;
        result.Amount = item.Amount * 0.01M;
        result.OfferAmount = item.OfferAmount * 0.01M;

        return result;
    }
}
