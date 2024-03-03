using FoodShop.Api.Workflow.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt => {
    mt.AddSagaStateMachine<OrderStateMachine, OrderStateMachineState>().EntityFrameworkRepository(c => {
        c.AddDbContext<DbContext, OrderStateMachineDbContext>((p, b) => {
            b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        c.ConcurrencyMode = ConcurrencyMode.Pessimistic;
    });

    mt.AddDelayedMessageScheduler();

    mt.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        configurator.UseDelayedMessageScheduler();

        configurator.ConfigureEndpoints(context);
    });

    mt.AddConsumers(Assembly.GetExecutingAssembly());
});

//TODO Does it really need?
builder.Services.AddDbContext<OrderStateMachineDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderStateMachineDbContext>())
{
    db.Database.EnsureCreated();
}

app.Run();
