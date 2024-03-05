namespace FoodShop.MessageContracts.Warehouse;

public record CreateParcel
(
    Guid OrderId
);


public record ParcelCreated (
    Guid OrderId,
    Guid ParcelId
);