using MassTransit;

//Is this a bug? https://stackoverflow.com/questions/65706167/weird-no-ip-address-could-be-resolved-in-rabbitmq-net-client
RabbitMQ.Client.ConnectionFactory.DefaultAddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(c =>
{
    c.AddConsumers(typeof(Program).Assembly);
    c.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["Messaging:RabbitMq:Host"], builder.Configuration["Messaging:RabbitMq:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["Messaging:RabbitMq:Username"]);
            h.Password(builder.Configuration["Messaging:RabbitMq:Password"]);
        });
        configurator.ConfigureEndpoints(context);
    });
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

app.Run();
