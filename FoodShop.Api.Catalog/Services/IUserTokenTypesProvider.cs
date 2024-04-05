using FoodShop.Api.Catalog.Commands;
using MediatR;
using System.Security.Claims;

namespace FoodShop.Api.Catalog.Services;

public interface IUserTokenTypesProvider
{
    Task<IEnumerable<string>> GetUserTokenTypes();
}


//TODO Consider merge this class with UserTokenTypesRequestHandler
public class HttpContextUserTokenTypesProvider(IHttpContextAccessor _httpContextAccessor, IMediator _mediator) : IUserTokenTypesProvider
{
    public async Task<IEnumerable<string>> GetUserTokenTypes()
    {
        var principal = _httpContextAccessor.HttpContext!.User;
        var isAnonymous = principal.Claims.Any(c => c.Type == ClaimTypes.Anonymous);
        if (isAnonymous)
        {
            return Array.Empty<string>();
        }

        var userName = principal.Identity?.Name;
        if(userName == null)
        {
            return Array.Empty<string>();
        }

        var response = await _mediator.Send(new UserTokenTypesRequest() { UserName = userName });
        return response.TokenTypes!;
    }
}
