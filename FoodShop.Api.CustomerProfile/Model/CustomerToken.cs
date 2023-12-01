namespace FoodShop.Api.CustomerProfile.Model;

public class CustomerToken
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string TokenTypeCode { get; set; }
    public CustomerTokenType? TokenType { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
