namespace FoodShop.Api.Basket.Model;

public class BasketItemsCollection : Dictionary<string, int>
{
    public void AddQuantity(string productId, int qty = 1)
    {
        int value;
        TryGetValue(productId, out value);

        value += qty;

        if (value > 0)
        {
            this[productId] = value;
        }
        else
        {
            Remove(productId);
        }
    }

    public void SetQuantity(string productId, int qty)
    {
        if (qty > 0)
        {
            this[productId] = qty;
        }
        else
        {
            Remove(productId);
        }
    }
}
