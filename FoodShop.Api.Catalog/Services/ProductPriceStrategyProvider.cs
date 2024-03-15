using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Catalog.Services;

/// <summary>
/// Finds price strategy for a product for giving token types
/// </summary>
public interface IProductPriceStrategyProvider
{
    ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds);
}

public class ProductPriceStrategyProvider : IProductPriceStrategyProvider
{
    private readonly IDbContextFactory<FoodShopDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    public const string ProductPriceStrategiesCacheKey = "PRODUCT_PRICE_STRATEGIES";

    public ProductPriceStrategyProvider(IDbContextFactory<FoodShopDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    public ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds)
    {
        ProductPriceStrategyLink result = ProductPriceStrategyLink.Default;

        var strategies = GetStrategies();

        var fullTokenTypeIds = tokenTypeIds.Append(null).Distinct();
        foreach (var tokenTypeId in fullTokenTypeIds)
        {
            foreach (var referenceId in product.ExtractEntityReferences())
            {
                var key = new StrategyKey(tokenTypeId, referenceId);
                if (strategies.TryGetValue(key, out var value))
                {
                    if (value.Priority > result.Priority)
                    {
                        result = value;
                    }
                }
            }
        }

        return result;
    }

    private Dictionary<StrategyKey, ProductPriceStrategyLink> GetStrategies()
    {
        if (_cache.TryGetValue(ProductPriceStrategiesCacheKey, out Dictionary<StrategyKey, ProductPriceStrategyLink>? value))
        {
            return value!;
        }
        else
        {
            try
            {
                _cacheLock.Wait();

                if (_cache.TryGetValue(ProductPriceStrategiesCacheKey, out value))
                {
                    return value!;
                }
                else
                {
                    using var db = _dbContextFactory.CreateDbContext();

                    value = db.ProductPriceStrategyLinks.AsNoTracking()
                        .Include(s => s.ProductPriceStrategy)
                        .ToDictionary(s => s.MapToStrategyKey());

                    _cache.Set(ProductPriceStrategiesCacheKey, value, new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10)
                    });

                    return value;
                }
            }
            finally
            {
                _cacheLock.Release();
            }
        }
    }
}




internal record struct EntityReference(
   EntityTypeCode ReferenceType,
   int ReferenceId
);

internal record struct StrategyKey(
    string? TokenTypeCode,
    EntityReference EntityReference
);

internal static class ProductPriceStrategyLinkExtensions
{
    public static StrategyKey MapToStrategyKey(this ProductPriceStrategyLink productPriceStrategyLink) =>
        new StrategyKey(productPriceStrategyLink.TokenTypeCode, new EntityReference(productPriceStrategyLink.ReferenceType, productPriceStrategyLink.ReferenceId));
}

internal static class ProductExtensions
{
    public static IEnumerable<EntityReference> ExtractEntityReferences(this Product product)
    {
        yield return new EntityReference(EntityTypeCode.Product, product.Id);

        if (product.Tags != null)
        {
            foreach (var tag in product.Tags)
            {
                yield return new EntityReference(EntityTypeCode.Tag, tag.TagId);
            }
        }

        if (product.CategoryId.HasValue)
        {
            yield return new EntityReference(EntityTypeCode.ProductCategory, product.CategoryId.Value);
        }

        if (product.BrandId.HasValue)
        {
            yield return new EntityReference(EntityTypeCode.Brand, product.BrandId.Value);
        }
    }
}