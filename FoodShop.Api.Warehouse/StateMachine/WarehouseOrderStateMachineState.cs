using MassTransit;

namespace FoodShop.Api.Warehouse.StateMachine;

public class WarehouseOrderStateMachineState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public Guid ParcelId { get; set; }
    public Guid? WarehouseCreateParcelTimeoutScheduleToken { get; set; }
}
