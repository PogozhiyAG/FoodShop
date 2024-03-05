namespace FoodShop.MessageContracts.Order;


public record OrderPaid(
    Guid OrderId,
    DateTime Moment
);

public record OrderCompleted(
    Guid OrderId,
    DateTime Moment
);



public record OrderItem
(
    string ProductId,
    int Quantity
);


public record DeliveryInfo
(
    string Address,
    DateTime TimeSlotFrom,
    DateTime TimeSlotTo,
    string? ContactPhone,
    string? ContactName
);


public record CreateOrder
(
    string Authorization,
    List<OrderItem> Items,
    DeliveryInfo? Delivery,
    string? Description
);

public record CreateOrderResponse
(
    Guid Id
);


