namespace FoodShop.Api.Basket.Model;

public class BasketItemsCollection : Dictionary<string, int>
{
    public void SetQuantity(string productId, int qty)
    {
        SetPositionQuantity(productId, qty);
    }

    public void AddQuantity(string productId, int qty = 1)
    {
        TryGetValue(productId, out var value);
        value += qty;
        SetPositionQuantity(productId, value);
    }

    private void SetPositionQuantity(string productId, int qty)
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
