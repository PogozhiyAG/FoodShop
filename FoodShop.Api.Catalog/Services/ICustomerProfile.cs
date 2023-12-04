using Azure.Core;
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

    public CustomerProfile(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IEnumerable<string>> GetTokenTypes(string userName)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var url = _configuration["ApiUrls:FoodShop.Api.CustomerProfile"] + "/" + userName;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Headers =
            {
                { HeaderNames.Authorization, GetAuthorizationHeaderValue() }
            }
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(stream);
        return result;
    }

    private string GetAuthorizationHeaderValue()
    {
        return "Bearer " + _configuration["JWT:Token"];
    }
}
