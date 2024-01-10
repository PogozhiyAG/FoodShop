namespace FoodShop.Api.Order.Dto;

public class CreateOrderRequest
{
    public List<OrderItemDto> Items { get; set; } = [];
    public DeliveryInfoDto? Delivery { get; set; }
    public string? Description { get; set; }
}
