using FoodShop.Api.Order.Dto.Catalog;
using Microsoft.Net.Http.Headers;

namespace FoodShop.Api.Order.Services;

public interface IProductCatalog
{
    Task<List<ProductBatchInfo>> GetProductBatchInfos(IEnumerable<(string, int)> productBatchRequests);
}


public class ProductCatalog : IProductCatalog
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ProductCatalog(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<List<ProductBatchInfo>> GetProductBatchInfos(IEnumerable<(string, int)> productBatchRequests)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var url = _configuration["ApiUrls:FoodShop.Api.Catalog"];

        var request = new ProductBatchCalculationRequest() {
            Items = productBatchRequests.ToDictionary(i => i.Item1, i => i.Item2)
        };

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Headers =
            {
                { HeaderNames.Authorization, _httpContextAccessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault() }
            },
            Content = JsonContent.Create(request)
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        var result = await httpResponseMessage.Content.ReadFromJsonAsync<List<ProductBatchInfo>>();

        return result;
    }
}

public class ProductBatchCalculationRequest
{
    public Dictionary<string, int> Items { get; set; } = new();

}