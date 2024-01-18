using System.Text.Json.Serialization;

namespace FoodShop.Api.Order.Model;

public class OrderCalculationProperty
{
    public Guid OrderCalculationId { get; set; }
    [JsonIgnore]
    public OrderCalculation OrderCalculation { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}
