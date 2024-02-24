namespace FoodShop.MessageContracts.Order;

public record OrderCreated (
    Guid OrderId,
    DateTime Moment
);

public record OrderPaid(
    Guid OrderId,
    DateTime Moment
);