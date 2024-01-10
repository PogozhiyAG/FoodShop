namespace FoodShop.Api.Order.Model;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}
