﻿using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Catalog.Services;

public interface IProductPriceStrategyProvider
{
    ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<int> tokenTypeIds);
}

public class ProductPriceStrategyProvider : IProductPriceStrategyProvider
{
    private readonly FoodShopDbContext _context;
    private readonly IMemoryCache _cache;

    public ProductPriceStrategyProvider(FoodShopDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
        var strategies = GetTypedStrategies();
    }

    public ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<int> tokenTypeIds)
    {
        return GetTypedStrategy(product, tokenTypeIds);
    }


    public ProductPriceStrategyLink GetTypedStrategy(Product product, IEnumerable<int> tokenTypeIds)
    {
        ProductPriceStrategyLink result = ProductPriceStrategyLink.Default;

        var strategies = GetTypedStrategies();

        foreach (var tokenTypeId in tokenTypeIds)
        {
            foreach (var referenceId in GetTypedReferencesFromProduct(product))
            {
                var key = new ValueTuple<int, EntityTypeCode, int>(tokenTypeId, referenceId.Item1, referenceId.Item2);
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

    private Dictionary<(int, EntityTypeCode, int), ProductPriceStrategyLink> GetTypedStrategies()
    {
        if (_cache.TryGetValue(nameof(GetTypedStrategies), out Dictionary<(int, EntityTypeCode, int), ProductPriceStrategyLink>? value))
        {
            return value!;
        }

        var result = _context.ProductPriceStrategyLinks.AsNoTracking()
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .ToDictionary(s => new ValueTuple<int, EntityTypeCode, int>(s.TokenTypeId.HasValue ? s.TokenTypeId.Value : 0, s.ReferenceType, s.ReferenceId));

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