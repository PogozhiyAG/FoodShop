using System.Security.Claims;

namespace FoodShop.Api.Catalog.Services;

public interface IUserTokenTypesProvider
{
    Task<IEnumerable<string>> GetUserTokenTypes();
}


public class HttpContextUserTokenTypesProvider : IUserTokenTypesProvider
{
    private readonly ICustomerProfile _customerProfile;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserTokenTypesProvider(ICustomerProfile customerProfile, IHttpContextAccessor httpContextAccessor)
    {
        _customerProfile = customerProfile;
        _httpContextAccessor = httpContextAccessor;
    }

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

        return await _customerProfile.GetTokenTypes(userName);
    }
}
