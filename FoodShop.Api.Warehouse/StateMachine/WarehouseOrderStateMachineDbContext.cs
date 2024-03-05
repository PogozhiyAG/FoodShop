using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace FoodShop.Api.Warehouse.StateMachine;

public class WarehouseOrderStateMachineDbContext : SagaDbContext
{
    public WarehouseOrderStateMachineDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new StateMachineMap(); }
    }
}


public class StateMachineMap : SagaClassMap<WarehouseOrderStateMachineState>
{

}