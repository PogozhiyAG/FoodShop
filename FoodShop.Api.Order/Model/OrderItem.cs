using System.Text.Json.Serialization;

namespace FoodShop.Api.Order.Model;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Order Order { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public ICollection<OrderCalculation> OrderCalculations { get; set; }
}
