namespace FoodShop.Core.Models;

public class BasketItem : EntityBase
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public Decimal Price { get; set; }
}
