using MassTransit;

namespace FoodShop.Api.Workflow.Order;

public class OrderStateMachineState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
}
