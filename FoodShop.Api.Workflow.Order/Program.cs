using FoodShop.Api.Workflow.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Reflection;

//Is this a bug? https://stackoverflow.com/questions/65706167/weird-no-ip-address-could-be-resolved-in-rabbitmq-net-client
ConnectionFactory.DefaultAddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
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
        configurator.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });

        configurator.UseDelayedMessageScheduler();

        configurator.ConfigureEndpoints(context);
    });

    mt.AddConsumers(Assembly.GetExecutingAssembly());
});

//TODO Is it really necessary?
builder.Services.AddDbContext<OrderStateMachineDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




var app = builder.Build();

app.UseCors("AllowAll");

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
