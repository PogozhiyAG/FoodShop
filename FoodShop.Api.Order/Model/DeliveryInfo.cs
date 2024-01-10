namespace FoodShop.Api.Order.Model;

public class DeliveryInfo
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public DateTime TimeSlotFrom { get; set; }
    public DateTime TimeSlotTo { get; set; }
    public string ContactPhone { get; set; }
    public string ContactName { get; set; }
}
