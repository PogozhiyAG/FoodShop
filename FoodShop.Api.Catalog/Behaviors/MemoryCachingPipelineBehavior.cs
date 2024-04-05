using FoodShop.Api.Catalog.Options;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FoodShop.Api.Catalog.Behaviors;

public class MemoryCachingPipelineBehavior<TRequest, TResponse> (
    IMemoryCache _cache,
    IOptions<MemoryCachingBehaviorOptions<TRequest>> _options
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private SemaphoreSlim cacheLock = new SemaphoreSlim(1, 1);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_options.Value.GetCacheKey == null)
        {
            return await next().ConfigureAwait(false);
        }

        var key = _options.Value.GetCacheKey(request);

        if (_cache.TryGetValue(key, out Task<TResponse>? responseTask) && !responseTask!.IsFaulted)
        {
            return await responseTask.ConfigureAwait(false);
        }

        try
        {
            await cacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            if (_cache.TryGetValue(key, out responseTask) && !responseTask!.IsFaulted)
            {
                return await responseTask.ConfigureAwait(false);
            }

            var wrapper = async () =>
            {
                await Task.Yield();
                return await next();
            };

            responseTask = wrapper();

            var cacheOptions = _options.Value.GetMemoryCacheEntryOptions!(request);

            //The task here is not a result. This is data in cache.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _cache.Set(key, responseTask, cacheOptions);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        finally
        {
            cacheLock.Release();
        }

        return await responseTask.ConfigureAwait(false);
    }
}
