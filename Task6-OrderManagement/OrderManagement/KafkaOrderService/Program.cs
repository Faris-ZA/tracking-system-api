using Microsoft.EntityFrameworkCore;
using KafkaOrderService.Clients;
using KafkaOrderService.Data;
using KafkaOrderService.Repositories;
using KafkaOrderService.Services;
using KafkaOrderService.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ILocalCustomerRepository, LocalCustomerRepository>();
builder.Services.AddScoped<IKafkaOrderService, KafkaOrderService.Services.KafkaOrderService>();

builder.Services.AddHostedService<CustomerEventsConsumer>();
builder.Services.AddHostedService<InventoryEventsConsumer>();
builder.Services.AddSingleton<IOrderEventProducer, OrderEventProducer>();

builder.Services.AddHttpClient<ICustomerClient, CustomerClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:CustomerService"]!);
});

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:InventoryService"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



