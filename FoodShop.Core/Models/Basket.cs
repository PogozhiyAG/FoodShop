namespace FoodShop.Core.Models;

public class Basket: EntityBase
{
    public string OwnerId { get; set; }
    public List<BasketItem> Items { get; set; } = new ();
    public void RemoveEmptyItems()
    {
        Items.RemoveAll(i => i.Quantity == 0);
    }
}
