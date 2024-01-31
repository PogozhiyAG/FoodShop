namespace FoodShop.Api.Order.Model;

public enum OrderStatus
{
    Draft,
    Created,
    Checkout,
    Confirmed,
    Paid,
    Delivery,
    Closed
}
