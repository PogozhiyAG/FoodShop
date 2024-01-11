namespace FoodShop.Api.Order.Model;

public class OrderCalculation
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public string TypeCode { get; set; }
    public DateTime CreateDate { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public ICollection<OrderCalculationProperty> Properties { get; set; }
}


public static class OrderCalculationTypeCodes
{
    public const string Product = "P";
    public const string Delivery = "D";
    public const string Service = "S";
    public const string ProductDiscount = "PD";
    public const string OrderDiscount = "OD";
}