using FoodShop.Api.Catalog.Commands;
using MediatR;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace FoodShop.Api.Catalog.CommandHandlers;

public class UserTokenTypesRequestHandler (
    IHttpClientFactory _httpClientFactory,
    IConfiguration _configuration
 ) : IRequestHandler<UserTokenTypesRequest, UserTokenTypesResponse>
{
    public const string CustomerProfileUrlConfigurationKey = "ApiUrls:FoodShop.Api.CustomerProfile";
    public const string JwtTokenConfigurationKey = "JWT:Token";

    public async Task<UserTokenTypesResponse> Handle(UserTokenTypesRequest request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var url = _configuration[CustomerProfileUrlConfigurationKey] + "/" + request.UserName;

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
        {
            Headers =
            {
                { HeaderNames.Authorization, "Bearer " + _configuration[JwtTokenConfigurationKey] }
            }
        };

        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        using var stream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(stream, cancellationToken: cancellationToken);

        return new UserTokenTypesResponse()
        {
            TokenTypes = result
        };
    }
}
