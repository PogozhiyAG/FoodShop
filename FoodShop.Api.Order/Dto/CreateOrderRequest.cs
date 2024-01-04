namespace FoodShop.Api.Order.Dto
{
    public class CreateOrderRequest
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public string? Description { get; set; }
    }
}
