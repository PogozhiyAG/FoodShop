using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodShop.Api.Workflow.Order;

public class OrderStateMachineDbContext : SagaDbContext
{
    public OrderStateMachineDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new StateMachineMap(); }
    }
}


public class StateMachineMap : SagaClassMap<OrderStateMachineState>
{
    protected override void Configure(EntityTypeBuilder<OrderStateMachineState> entity, ModelBuilder model)
    {

    }
}