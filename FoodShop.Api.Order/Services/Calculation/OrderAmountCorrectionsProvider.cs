using FoodShop.Api.Order.Data;
using FoodShop.Api.Order.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Order.Services.Calculation;

public interface IOrderAmountCorrectionsProvider
{
    Task<IEnumerable<OrderAmountCorrection>> GetCorrections(IEnumerable<string> tokenTypeIds);
}

public class OrderAmountCorrectionsProvider : IOrderAmountCorrectionsProvider
{
    private readonly IDbContextFactory<OrderDbContext> _dbContextFactory;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    public const string CACHE_KEY = nameof(GetCorrectionsDictionary);

    public OrderAmountCorrectionsProvider(IDbContextFactory<OrderDbContext> dbContextFactory, IMemoryCache cache)
    {
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }

    public async Task<IEnumerable<OrderAmountCorrection>> GetCorrections(IEnumerable<string> tokenTypeIds)
    {
        var correctionsDictionary = await GetCorrectionsDictionary();
        Dictionary<string, OrderAmountCorrection> scopes = new Dictionary<string, OrderAmountCorrection>();
        var fullTokenTypeIds = tokenTypeIds.Append(string.Empty).Distinct();

        foreach (var tokenTypeId in fullTokenTypeIds)
        {
            if (correctionsDictionary.TryGetValue(tokenTypeId, out var list))
            {
                foreach (var item in list)
                {
                    var key = item.Scope ?? string.Empty;
                    if (scopes.TryGetValue(key, out var saved))
                    {
                        if(item.Priority > saved.Priority)
                        {
                            scopes[key] = item;
                        }
                    }
                    else
                    {
                        scopes[key] = item;
                    }
                }
            }
        }

        return scopes.Values.OrderBy(x => x.Priority);
    }

    private async Task<Dictionary<string, List<OrderAmountCorrection>>> GetCorrectionsDictionary()
    {

        if (_cache.TryGetValue(CACHE_KEY, out Dictionary<string, List<OrderAmountCorrection>>? value))
        {
            return value!;
        }
        else
        {
            try
            {
                _cacheLock.Wait();

                if (_cache.TryGetValue(CACHE_KEY, out value))
                {
                    return value!;
                }
                else
                {
                    using var db = _dbContextFactory.CreateDbContext();

                    value = await db.OrderAmountCorrections.AsNoTracking()
                        .GroupBy(c => c.TokenTypeCode ?? string.Empty)
                        .ToDictionaryAsync(g => g.Key, g => g.ToList());

                    _cache.Set(CACHE_KEY, value, new MemoryCacheEntryOptions()
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
