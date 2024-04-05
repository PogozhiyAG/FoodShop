using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Catalog.Options;

public class MemoryCachingBehaviorOptions<TRequest>
{
    public Func<TRequest, string>? GetCacheKey { get; set; }
    public Func<TRequest, MemoryCacheEntryOptions>? GetMemoryCacheEntryOptions { get; set; }
}
