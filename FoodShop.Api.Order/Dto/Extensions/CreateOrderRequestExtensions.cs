namespace FoodShop.Api.Order.Dto.Extensions;

public static class CreateOrderRequestExtensions
{
    public static Model.Order ToOrder(this CreateOrderRequest createOrderRequest, Action<Model.Order> configure)
    {
        var now = DateTime.UtcNow;

        Model.Order order = new()
        {
            Id = Guid.NewGuid(),
            CreateDate = DateTime.UtcNow,
            Description = createOrderRequest.Description
        };

        foreach (var requestOrderItem in createOrderRequest.Items)
        {
            order.Items.Add(new()
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Order = order,
                ProductId = requestOrderItem.ProductId,
                Quantity = requestOrderItem.Quantity
            });
        }

        if (createOrderRequest.Delivery != null)
        {
            order.DeliveryInfo = new()
            {
                Id = Guid.NewGuid(),
                Address = createOrderRequest.Delivery.Address,
                ContactName = createOrderRequest.Delivery.ContactName,
                ContactPhone = createOrderRequest.Delivery.ContactPhone,
                TimeSlotFrom = createOrderRequest.Delivery.TimeSlotFrom,
                TimeSlotTo = createOrderRequest.Delivery.TimeSlotTo
            };
        }

        if (configure != null)
        {
            configure(order);
        }

        return order;
    }
}
