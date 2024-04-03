using FoodShop.Api.Catalog.Services;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FoodShop.Api.Catalog.Behaviors;

public class CachingPipelineBehavior<TRequest, TResponse> (
    IMemoryCache _cache,
    IOptions<CachingBehaviorOptions<TRequest, TResponse>> _options
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(_options.Value.GetCacheKey != null)
        {
            var key = _options.Value.GetCacheKey(request);
            if (_cache.TryGetValue(key, out TResponse? response))
            {
                return response!;
            }

            response = await next().ConfigureAwait(false);

            var cacheOptions = _options.Value.GetMemoryCacheEntryOptions(request, response);

            _cache.Set(key, response, cacheOptions);

            return response;
        }

        return await next().ConfigureAwait(false);
    }
}
