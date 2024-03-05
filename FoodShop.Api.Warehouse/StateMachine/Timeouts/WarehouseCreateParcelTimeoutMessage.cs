using MassTransit;

namespace FoodShop.Api.Warehouse.StateMachine.Timeouts;

public record WarehouseCreateParcelTimeoutMessage : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
}