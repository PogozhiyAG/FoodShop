using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Web.Services;

public interface IProductPriceStrategyProvider
{
    IEnumerable<ProductPriceStrategyLink> GetStrategyLinks(Product product, IEnumerable<Guid> tokenTypeIds);
}

public class ProductPriceStrategyProvider : IProductPriceStrategyProvider
{
    private readonly FoodShopDbContext _context;
    private readonly IMemoryCache _cache;

    public ProductPriceStrategyProvider(FoodShopDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public IEnumerable<ProductPriceStrategyLink> GetStrategyLinks(Product product, IEnumerable<Guid> tokenTypeIds)
    {
        yield return ProductPriceStrategyLink.Default;

        var strategyLinks = GetProductPriceStrategyLinksDictionary();

        foreach (var tokenTypeId in tokenTypeIds)
        {
            if (strategyLinks.TryGetValue(tokenTypeId, out var innerDir))
            {
                foreach (var reference in GetReferencesFromProduct(product))
                {
                    if (innerDir.TryGetValue(reference, out var strategyLink))
                    {
                        yield return strategyLink;
                    }
                }
            }
        }
    }

    private IEnumerable<Guid> GetReferencesFromProduct(Product product)
    {
        yield return product.Id;
        if (product.CategoryId.HasValue)
        {
            yield return product.CategoryId.Value;
        }
        foreach (var tagRef in product.Tags)
        {
            yield return tagRef.TagId;
        }
    }

    private Dictionary<Guid, Dictionary<Guid, ProductPriceStrategyLink>> GetProductPriceStrategyLinksDictionary()
    {
        if (_cache.TryGetValue(nameof(GetProductPriceStrategyLinksDictionary), out Dictionary<Guid, Dictionary<Guid, ProductPriceStrategyLink>> value))
        {
            return value!;
        }

        var strategyLinks = _context.ProductPriceStrategyLinks
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .AsNoTracking()
            .ToList();

        var result = new Dictionary<Guid, Dictionary<Guid, ProductPriceStrategyLink>>();

        foreach (var strategyLink in strategyLinks)
        {
            Guid tokenId = strategyLink.TokenTypeId ?? Guid.Empty;
            if (!result.TryGetValue(tokenId, out var tokenDic))
            {
                result[tokenId] = tokenDic = new Dictionary<Guid, ProductPriceStrategyLink>();
            }

            tokenDic[strategyLink.ReferenceId] = strategyLink;
        }

        _cache.Set(nameof(GetProductPriceStrategyLinksDictionary), result, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

        return result;
    }
}
