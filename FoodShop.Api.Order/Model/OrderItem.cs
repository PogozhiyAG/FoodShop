using System.Text.Json.Serialization;

namespace FoodShop.Api.Order.Model;

public class OrderItem
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Order Order { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    [JsonIgnore]
    public ICollection<OrderCalculation> OrderCalculations { get; set; }
}
