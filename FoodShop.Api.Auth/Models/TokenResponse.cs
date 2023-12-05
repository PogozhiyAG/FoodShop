namespace FoodShop.Api.Auth.Models
{
    public class TokenResponse
    {
        public required string Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
