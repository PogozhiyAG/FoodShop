using System.Security.Claims;

namespace FoodShop.Api.Order.Services;

public interface IAuthenticationContext
{
    public ClaimsPrincipal? User { get; set; }
    public string? Token{ get; set; }
}

public class AuthenticationContext : IAuthenticationContext
{
    public ClaimsPrincipal? User { get; set; }
    public string? Token { get; set; }
}