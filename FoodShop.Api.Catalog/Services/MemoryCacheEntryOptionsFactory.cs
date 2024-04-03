using Microsoft.Extensions.Caching.Memory;

namespace FoodShop.Api.Catalog.Services;

public class CachingBehaviorOptions<TRequest, TResponse>
{
    public Func<TRequest, string>  GetCacheKey { get; set; }
    public Func<TRequest, TResponse, MemoryCacheEntryOptions> GetMemoryCacheEntryOptions { get; set; }

}


