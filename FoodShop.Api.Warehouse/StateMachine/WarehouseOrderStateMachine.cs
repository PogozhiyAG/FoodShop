using FoodShop.Api.Warehouse.StateMachine.Timeouts;
using FoodShop.MessageContracts.Warehouse;
using MassTransit;

namespace FoodShop.Api.Warehouse.StateMachine;

public class WarehouseOrderStateMachine : MassTransitStateMachine<WarehouseOrderStateMachineState>
{
    public Event<CreateParcel> CreateParcelCommand  {get; set; }

    public State WaitingForParcelCreated { get; set; }

    public Schedule<WarehouseOrderStateMachineState, WarehouseCreateParcelTimeoutMessage> WarehouseCreateParcelTimeoutSchedule { get; set; }



    public WarehouseOrderStateMachine()
    {
        InstanceState(s => s.CurrentState);

        Event(() => CreateParcelCommand, c => c.CorrelateBy<Guid>(s => s.OrderId, z => z.Message.OrderId).SelectId(ctx => ctx.Message.OrderId));

        Schedule(() => WarehouseCreateParcelTimeoutSchedule, s => s.WarehouseCreateParcelTimeoutScheduleToken, c => { c.Received = p => p.CorrelateById(s => s.Message.CorrelationId); });


        Initially(
             When(CreateParcelCommand)
             .Then(ctx => {
                 ctx.Saga.OrderId = ctx.Message.OrderId;
             })
            .Schedule(WarehouseCreateParcelTimeoutSchedule,
                ctx => ctx.Init<WarehouseCreateParcelTimeoutMessage>(new WarehouseCreateParcelTimeoutMessage() { CorrelationId = ctx.Saga.CorrelationId, ParcelId = Guid.NewGuid() }),
                ctx => TimeSpan.FromSeconds(10)
            )
            .TransitionTo(WaitingForParcelCreated)
        );


        During(WaitingForParcelCreated,
            When(WarehouseCreateParcelTimeoutSchedule!.Received)
            .Then(ctx =>
            {
                ctx.Saga.ParcelId = ctx.Message.ParcelId;
            })
            .Publish(ctx => new ParcelCreated(
                OrderId: ctx.Saga.OrderId,
                ParcelId: ctx.Saga.ParcelId
            ))
            .Finalize()
        );

    }
}
