namespace FoodShop.Api.Auth.Models;

public class LoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
