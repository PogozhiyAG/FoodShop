using FoodShop.MessageContracts.Order;
using FoodShop.MessageContracts.Warehouse;
using MassTransit;

namespace FoodShop.Api.Workflow.Order;

public class OrderStateMachine : MassTransitStateMachine<OrderStateMachineState>
{
    public Event<OrderPaid> OrderPaidEvent { get; set; }
    public Event<ParcelCreated> ParcelCreatedEvent { get; set; }

    public State Created { get; set; }
    public State WaitingForParcel { get; set; }
    public State SendNotification { get; set; }

    public OrderStateMachine()
    {
        InstanceState(s => s.CurrentState);

        Event(() => OrderPaidEvent, c => c.CorrelateBy<Guid>(s => s.OrderId, z => z.Message.OrderId).SelectId(ctx => ctx.Message.OrderId));
        Event(() => ParcelCreatedEvent, c => c.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
             When(OrderPaidEvent)
             .Then(ctx =>
             {
                 ctx.Saga.OrderId = ctx.Message.OrderId;
             })
            .Publish(ctx => new CreateParcel(
                OrderId: ctx.Saga.OrderId
             ))
            .TransitionTo(WaitingForParcel)
        );

        During(WaitingForParcel,
            When(ParcelCreatedEvent)
            .TransitionTo(SendNotification)
        );

        WhenEnter(SendNotification, b => b
            .Then(ctx => Console.WriteLine($"SendNotification for order {ctx.Saga.OrderId}"))
            .Finalize()
        );
    }
}
