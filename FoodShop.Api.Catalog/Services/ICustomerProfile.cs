using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace FoodShop.Api.Catalog.Services;


public interface ICustomerProfile
{
    Task<IEnumerable<string>> GetTokenTypes(string userName);
}


public class CustomerProfile: ICustomerProfile
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public CustomerProfile(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<IEnumerable<string>> GetTokenTypes(string userName)
    {
        var cacheKey = nameof(GetTokenTypes) + "/" + userName;

        if (_cache.TryGetValue(cacheKey, out IEnumerable<string>? value))
        {
            return value!;
        }

        var httpClient = _httpClientFactory.CreateClient();
        var url = _configuration["ApiUrls:FoodShop.Api.CustomerProfile"] + "/" + userName;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Headers =
            {
                { HeaderNames.Authorization, "Bearer " + _configuration["JWT:Token"] }
            }
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(stream);

        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30)
        });

        return result;
    }
}
