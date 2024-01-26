namespace FoodShop.Api.CustomerProfile.Model;

public class CustomerDeliveryInfo
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public string? Address { get; set; }
    public string? PostCode { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
}
