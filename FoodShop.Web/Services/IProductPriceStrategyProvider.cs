using FoodShop.Core.Models;
using FoodShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace FoodShop.Web.Services;

public interface IProductPriceStrategyProvider
{
    IEnumerable<ProductPriceStrategyLink> GetStrategyLinks(Product product, IEnumerable<int> tokenTypeIds);
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



    public ProductPriceStrategyLink GetStrategy(Product product, IEnumerable<int> tokenTypeIds)
    {
        var strategies = GetStrategies();

        foreach (var tokenTypeId in tokenTypeIds)
        {
            foreach (var referenceId in GetReferencesFromProduct(product))
            {
                var key = new ValueTuple<int, int>(tokenTypeId, referenceId);
                if(strategies.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
        }

        return ProductPriceStrategyLink.Default;
    }

    private Dictionary<(int, int), ProductPriceStrategyLink> GetStrategies()
    {
        if (_cache.TryGetValue(nameof(GetProductPriceStrategyLinks), out Dictionary<(int, int), ProductPriceStrategyLink>? value))
        {
            return value!;
        }

        var result = _context.ProductPriceStrategyLinks.AsNoTracking()
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .ToDictionary(s => new ValueTuple<int, int>(s.TokenTypeId.HasValue ? s.TokenTypeId.Value : 0, s.ReferenceId));

        _cache.Set(nameof(GetStrategies), result, new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        return result;
    }


    private IEnumerable<int> GetReferencesFromProduct(Product product)
    {
        yield return product.Id;

        foreach (var tag in product.Tags.Select(r => r.Tag))
        {
            yield return tag.Id;
        }
        if (product.CategoryId.HasValue)
        {
            yield return product.CategoryId.Value;
        }
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
        if (_cache.TryGetValue(nameof(GetProductPriceStrategyLinks), out Dictionary<(int, EntityTypeCode, int), ProductPriceStrategyLink>? value))
        {
            return value!;
        }

        var result = _context.ProductPriceStrategyLinks.AsNoTracking()
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .ToDictionary(s => new ValueTuple<int, EntityTypeCode, int>(s.TokenTypeId.HasValue ? s.TokenTypeId.Value : 0, s.ReferenceType, s.ReferenceId));

        _cache.Set(nameof(GetStrategies), result, new MemoryCacheEntryOptions()
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



    public IEnumerable<ProductPriceStrategyLink> GetStrategyLinks(Product product, IEnumerable<int> tokenTypeIds)
    {
        yield return GetTypedStrategy(product, tokenTypeIds);
        //yield return GetStrategy(product, tokenTypeIds);
        //yield return GetStrategyLink(product, tokenTypeIds);
        //yield return ProductPriceStrategyLink.Default;

        //var strategyLinks = GetProductPriceStrategyLinksDictionary();

        //foreach (var tokenTypeId in tokenTypeIds)
        //{
        //    if (strategyLinks.TryGetValue(tokenTypeId, out var innerDir))
        //    {
        //        foreach (var reference in GetReferencesFromProduct(product))
        //        {
        //            if (innerDir.TryGetValue(reference, out var strategyLink))
        //            {
        //                yield return strategyLink;
        //            }
        //        }
        //    }
        //}
    }



    private Dictionary<int, Dictionary<int, ProductPriceStrategyLink>> GetProductPriceStrategyLinksDictionary()
    {
        if (_cache.TryGetValue(nameof(GetProductPriceStrategyLinksDictionary), out Dictionary<int, Dictionary<int, ProductPriceStrategyLink>> value))
        {
            return value!;
        }

        var strategyLinks = _context.ProductPriceStrategyLinks
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .AsNoTracking()
            .ToList();

        var result = new Dictionary<int, Dictionary<int, ProductPriceStrategyLink>>();

        foreach (var strategyLink in strategyLinks)
        {
            int tokenId = strategyLink.TokenTypeId ?? 0;
            if (!result.TryGetValue(tokenId, out var tokenDic))
            {
                result[tokenId] = tokenDic = new Dictionary<int, ProductPriceStrategyLink>();
            }

            tokenDic[strategyLink.ReferenceId] = strategyLink;
        }

        _cache.Set(nameof(GetProductPriceStrategyLinksDictionary), result, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

        return result;
    }




    /*
     ////////////////////////////////////////////////////////////
    */




    public ProductPriceStrategyLink GetStrategyLink(Product product, IEnumerable<int> tokenTypeIds)
    {
        var links = GetProductPriceStrategyLinks();

        tokenTypeIds.Contains(0);

        var q = links
            .Where(l => l.ReferenceId == product.Id && (l.TokenType == null || tokenTypeIds.Contains(l.TokenTypeId!.Value)))
          //  .OrderByDescending(l => l.TokenType != null ? l.TokenType!.OfferPriority : 0)
            .FirstOrDefault();
        if (q != null)
        {
            return q;
        }


        foreach (var tagRef in product.Tags)
        {
            q = links
            .Where(l => l.ReferenceId == tagRef.TagId && (l.TokenType == null || tokenTypeIds.Contains(l.TokenTypeId!.Value)))
          //  .OrderByDescending(l => l.TokenType != null ? l.TokenType!.OfferPriority : 0)
            .FirstOrDefault();
            if (q != null)
            {
                return q;
            }
        }

        q = links
           .Where(l => product.CategoryId.HasValue && l.ReferenceId == product.CategoryId.Value && (l.TokenType == null || tokenTypeIds.Contains(l.TokenTypeId!.Value)))
           //.OrderByDescending(l => l.TokenType != null ? l.TokenType!.OfferPriority : 0)
           .FirstOrDefault();
        if (q != null)
        {
            return q;
        }



        return ProductPriceStrategyLink.Default;
    }


    private List<ProductPriceStrategyLink> GetProductPriceStrategyLinks()
    {
        if (_cache.TryGetValue(nameof(GetProductPriceStrategyLinks), out List<ProductPriceStrategyLink>? value))
        {
            return value!;
        }

        var result = _context.ProductPriceStrategyLinks.AsNoTracking()
            .Include(s => s.ProductPriceStrategy)
            .Include(s => s.TokenType)
            .ToList();


        _cache.Set(nameof(GetProductPriceStrategyLinks), result, new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        return result;
    }




}
