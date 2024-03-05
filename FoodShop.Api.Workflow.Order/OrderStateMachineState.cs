using MassTransit;

namespace FoodShop.Api.Workflow.Order;

public class OrderStateMachineState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
}
