namespace FoodShop.Api.Order.Model;

public class OrderPaymentIntent
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public required string PaymentIntentId { get; set; }
}
