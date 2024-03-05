using FoodShop.Api.Warehouse.StateMachine;
using FoodShop.BuildingBlocks.Configuration.Security;
using FoodShop.Order.Grpc;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

//Is this a bug? https://stackoverflow.com/questions/65706167/weird-no-ip-address-could-be-resolved-in-rabbitmq-net-client
RabbitMQ.Client.ConnectionFactory.DefaultAddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mt =>
{
    mt.AddSagaStateMachine<WarehouseOrderStateMachine, WarehouseOrderStateMachineState>().EntityFrameworkRepository(c => {
        c.AddDbContext<DbContext, WarehouseOrderStateMachineDbContext>((p, b) => {
            b.UseSqlServer(builder.Configuration.GetConnectionString("StateMachine"));
        });

        c.ConcurrencyMode = ConcurrencyMode.Pessimistic;
    });

    mt.AddDelayedMessageScheduler();

    mt.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["Messaging:RabbitMq:Host"], builder.Configuration["Messaging:RabbitMq:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["Messaging:RabbitMq:Username"]);
            h.Password(builder.Configuration["Messaging:RabbitMq:Password"]);
        });

        configurator.UseDelayedMessageScheduler();

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddFoodShopJwt();

//TODO Is it really necessary?
builder.Services.AddDbContext<WarehouseOrderStateMachineDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//gRPC
builder.Services
    .AddGrpcClient<OrderService.OrderServiceClient>(o =>
    {
        o.Address = new Uri(builder.Configuration["ApiUrls:FoodShop.Api.Order.Grpc"]!);
    })
    .AddCallCredentials((context, metadata, sp) =>
    {
        var token = builder.Configuration["JWT:Token"];
        metadata.Add(HeaderNames.Authorization, $"Bearer {token}");
        return Task.CompletedTask;
    })
    .ConfigureChannel(o =>
    {
        o.UnsafeUseInsecureChannelCallCredentials = true;
    });




var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapGrpcReflectionService();

using (var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<WarehouseOrderStateMachineDbContext>())
{
    db.Database.EnsureCreated();
}

app.Run();
