using FoodShop.Catalog.Grpc;
using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Catalog.Services;


[Authorize]
public class ProductCalculatorGrpcService : ProductCalculator.ProductCalculatorBase
{
    private readonly IDbContextFactory<FoodShopDbContext> _dbContextFactory;
    private readonly IProductPriceStrategyProvider _priceStrategyProvider;
    private readonly IUserTokenTypesProvider _userTokenTypesProvider;

    public ProductCalculatorGrpcService(IDbContextFactory<FoodShopDbContext> dbContextFactory, IProductPriceStrategyProvider priceStrategyProvider, IUserTokenTypesProvider userTokenTypesProvider)
    {
        _dbContextFactory = dbContextFactory;
        _priceStrategyProvider = priceStrategyProvider;
        _userTokenTypesProvider = userTokenTypesProvider;
    }

    public override async Task<ProductCalculationResponse> Calculate(ProductCalculationRequest request, ServerCallContext context)
    {
        var tokenTypes = await _userTokenTypesProvider.GetUserTokenTypes();

        using var db = await _dbContextFactory.CreateDbContextAsync();

        var productDic = request.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
        var productIds = productDic.Keys.ToList();

        var items = db.Products
            .SetupProductQuery()
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.ToProductCalculationResponseItem(
                _priceStrategyProvider.GetStrategyLink(p, tokenTypes),
                productDic[p.Id]
             ))
            .ToList();

        var result = new ProductCalculationResponse();
        result.Items.AddRange(items);
        return result;
    }
}


public static class ProductQueryExtensions
{
    public static IQueryable<Product> SetupProductQuery(this IQueryable<Product> products) => products
        .AsNoTracking()
        .Include(p => p.Tags).ThenInclude(t => t.Tag)
        .Include(p => p.Brand)
        .Include(p => p.Category);
}


public static class ProductMappingExtensions
{
    public static ProductCalculationResponseItem ToProductCalculationResponseItem(this Product product, ProductPriceStrategyLink offerLink, int quantity)
    {
        ProductCalculationResponseItem result = new();

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
        result.OfferPrice = Convert.ToInt32(Math.Round(offerLink.ProductPriceStrategy.GetAmount(product.Price, 1) * 100));
        result.Quantity = quantity;
        result.Amount = Convert.ToInt32(Math.Round(ProductPriceStrategy.Default.GetAmount(product.Price, quantity) * 100));
        result.OfferAmount = Convert.ToInt32(Math.Round(offerLink.ProductPriceStrategy.GetAmount(product.Price, quantity) * 100));

        return result;
    }

}