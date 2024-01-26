using System.Text.Json.Serialization;

namespace FoodShop.Api.Order.Model;

public class OrderCalculation
{
    [JsonIgnore]
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Order Order { get; set; }
    public Guid? OrderItemId { get; set; }
    [JsonIgnore]
    public OrderItem? OrderItem { get; set; }
    public string TypeCode { get; set; }
    public DateTime CreateDate { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public ICollection<OrderCalculationProperty> Properties { get; set; } = new List<OrderCalculationProperty>();
}


public static class OrderCalculationTypeCodes
{
    public const string Product = "P";
    public const string ProductDiscount = "PD";
    public const string Delivery = "D";
    public const string DeliveryDiscount = "DD";
    public const string Service = "S";
    public const string ServiceDiscount = "SD";
    public const string OrderVolumeDiscount = "OVD";
}