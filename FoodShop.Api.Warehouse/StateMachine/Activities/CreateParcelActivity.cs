using FoodShop.Api.Warehouse.Commands;
using MassTransit;
using MediatR;

namespace FoodShop.Api.Warehouse.StateMachine.Activities;

public class CreateParcelActivity(IMediator _mediator) : IStateMachineActivity<WarehouseOrderStateMachineState>
{

    private async Task CreateParcel(BehaviorContext<WarehouseOrderStateMachineState> context)
    {
        var commandResult = await _mediator.Send(new CreateParcelRequest() { OrderId = context.Saga.OrderId });
        context.Saga.ParcelId = commandResult.ParcelId;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<WarehouseOrderStateMachineState> context, IBehavior<WarehouseOrderStateMachineState> next)
    {
        await CreateParcel(context);

        await next.Execute(context).ConfigureAwait(false);
    }

    public async Task Execute<T>(BehaviorContext<WarehouseOrderStateMachineState, T> context, IBehavior<WarehouseOrderStateMachineState, T> next) where T : class
    {
        await CreateParcel(context);

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<WarehouseOrderStateMachineState, TException> context, IBehavior<WarehouseOrderStateMachineState> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public Task Faulted<T, TException>(BehaviorExceptionContext<WarehouseOrderStateMachineState, T, TException> context, IBehavior<WarehouseOrderStateMachineState, T> next)
        where T : class
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {

    }
}
