using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Catalog.Services;

public interface IProductPriceStrategyProvider
{
    ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds);
}

public class ProductPriceStrategyProvider : IProductPriceStrategyProvider
{
    private readonly IDbContextFactory<FoodShopDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;

    public ProductPriceStrategyProvider(IDbContextFactory<FoodShopDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    public ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<string> tokenTypeIds)
    {
        return GetTypedStrategy(product, tokenTypeIds);
    }


    public ProductPriceStrategyLink GetTypedStrategy(Product product, IEnumerable<string> tokenTypeIds)
    {
        ProductPriceStrategyLink result = ProductPriceStrategyLink.Default;

        var strategies = GetTypedStrategies();

        foreach (var tokenTypeId in tokenTypeIds)
        {
            foreach (var referenceId in GetTypedReferencesFromProduct(product))
            {
                var key = new ValueTuple<string, EntityTypeCode, int>(tokenTypeId, referenceId.Item1, referenceId.Item2);
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

    private Dictionary<(string, EntityTypeCode, int), ProductPriceStrategyLink> GetTypedStrategies()
    {
        if (_cache.TryGetValue(nameof(GetTypedStrategies), out Dictionary<(string, EntityTypeCode, int), ProductPriceStrategyLink>? value))
        {
            return value!;
        }

        using var db = _dbContextFactory.CreateDbContext();

        var result = db.ProductPriceStrategyLinks.AsNoTracking()
            .Include(s => s.ProductPriceStrategy)
            .ToDictionary(s => new ValueTuple<string, EntityTypeCode, int>(s.TokenTypeCode ?? "", s.ReferenceType, s.ReferenceId));

        _cache.Set(nameof(GetTypedStrategies), result, new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        return result;
    }

    private IEnumerable<ValueTuple<EntityTypeCode, int>> GetTypedReferencesFromProduct(Product product)
    {
        yield return (EntityTypeCode.Product, product.Id);

        foreach (var tag in product.Tags.Select(r => r.Tag))
        {
            yield return (EntityTypeCode.Tag, tag.Id);
        }

        if (product.CategoryId.HasValue)
        {
            yield return (EntityTypeCode.ProductCategory, product.CategoryId.Value);
        }

        if (product.BrandId.HasValue)
        {
            yield return (EntityTypeCode.Brand, product.BrandId.Value);
        }
    }
}