
namespace FoodShop.Api.Auth.Models;

public class RefreshRequest
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}
